namespace Services.Management.Administration.Executioner
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using Chains;
    using Chains.Play;
    using Chains.Play.Modules;
    using Chains.Play.Web;
    using Services.Communication.Protocol;
    using Services.Management.Administration.Server;
    using Services.Management.Administration.Worker;
    using Services.Management.Tracing;

    public sealed class WorkerExecutioner : IExecutioner, IDisposable
    {
        public readonly ExecutionMode ExecutionMode;
        public readonly StartWorkerData WorkerData;
        public readonly string Session;
        public readonly string ApiKey;

        private readonly IProcessExit processExit;
        private WorkUnitContext workUnitContext;
        private AdministrationContext adminContextServer;

        public object WrappedContext { get; private set; }

        public WorkUnitContext WorkUnitContext
        {
            get
            {
                return workUnitContext;
            }
        }

        public WorkerExecutioner(ExecutionMode executionMode, StartWorkerData workerData, string session = null, string apiKey = null, IProcessExit processExit = null)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;

            ExecutionMode = executionMode;
            WorkerData = workerData;
            Session = session;
            ApiKey = apiKey;
            this.processExit = processExit ?? new ProcessExit();
        }

        public void Execute()
        {
            try
            {
                switch (ExecutionMode)
                {
                    case ExecutionMode.AdministrationServer:
                    {
                        adminContextServer =
                            new ServerHost(WorkerData.AdminHost, WorkerData.AdminPort).Do(
                                new EnableAdminServer(hostProcessName: WorkerData.HostProcessFileName));

                        ConnectModules(
                            adminContextServer,
                            new object[]
                            {
                                workUnitContext, adminContextServer
                            },
                            WorkerData.Modules);

                        break;
                    }
                    case ExecutionMode.UpdateAdministrationServer:
                    {
                        var adminHost = WorkerData.AdminHost == IPAddress.Any.ToString() ? "localhost" : WorkerData.AdminHost;

                        using (
                            var adminServer =
                                new Client(adminHost, WorkerData.AdminPort).Do(new OpenConnection()))
                        {
                            adminServer
                                .Do(new Send(new CloseAdminForUpdate { Session = Session, ApiKey = ApiKey }))
                                .Do(new WaitUntilServerIsDown());
                        }

                        CopyFiles(AppDomain.CurrentDomain.BaseDirectory, Path.GetDirectoryName(WorkerData.DllPath));

                        // Run the previous process again in the same host/port
                        Process.Start(
                            new ProcessStartInfo
                            {
                                FileName = WorkerData.DllPath,
                                Arguments = GetProcessArguments(ExecutionMode.AdministrationServer),
                                WorkingDirectory = Path.GetDirectoryName(WorkerData.DllPath),
                                UseShellExecute = true
                            });
                        break;
                    }
                    case ExecutionMode.Worker:
                    {
                        // Needs to be attached to the object to finalize it before running any actions
                        workUnitContext = new WorkUnitContext(WorkerData, Session, ApiKey, processExit);

                        workUnitContext
                            .Do(new PrepareAssembliesInWorkUnit())
                            .Do(new StartWorkUnitContextServer())
                            .Do(new ConnectWorkUnitToAdmin());

                        WrappedContext = CreateHostedContext(workUnitContext);

                        if (workUnitContext.ContextServer != null)
                        {
                            workUnitContext.ContextServer.Do(new DelaySetHostedObject(WrappedContext));
                        }

                        workUnitContext.Do(new ConnectHostedObject(WrappedContext)).Do(new StartWorkUnit());

                        break;
                    }
                    case ExecutionMode.PrepareWorker:
                    {
                        using (
                            var adminServer =
                                new Client(WorkerData.AdminHost, WorkerData.AdminPort).Do(new OpenConnection()))
                        {
                            adminServer.Do(new Send(new PrepareWorkerProcessFiles(WorkerData) { Session = Session, ApiKey = ApiKey }));
                        }

                        break;
                    }
                    case ExecutionMode.StartWorkerFromAdmin:
                    {
                        using (
                            var adminServer =
                                new Client(WorkerData.AdminHost, WorkerData.AdminPort).Do(new OpenConnection()))
                        {
                            adminServer.Do(new Send(new StartWorkerProcess(WorkerData) { Session = Session, ApiKey = ApiKey }));
                        }

                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    workUnitContext.LogException(ex);
                    workUnitContext.ReportToAdmin();
                }
                catch
                {
                    new EmergencyLogger().SaveToFile(ex);
                }

                try
                {
                    Dispose();
                }
                catch
                {
                }

                throw;
            }
        }

        public string GetProcessArguments(ExecutionMode mode)
        {
            var workerDataSessionAndApiKey = new WorkerDataSessionAndApiKey
                                             {
                                                 AdminHost = WorkerData.AdminHost,
                                                 AdminPort = WorkerData.AdminPort,
                                                 ContextServerHost = WorkerData.ContextServerHost,
                                                 ContextServerPort = WorkerData.ContextServerPort,
                                                 ApiKey = ApiKey,
                                                 ContextType = WorkerData.ContextType,
                                                 DllPath = WorkerData.DllPath,
                                                 HostProcessFileName = WorkerData.HostProcessFileName,
                                                 Id = WorkerData.Id,
                                                 Modules = WorkerData.Modules,
                                                 Parameters = WorkerData.Parameters,
                                                 ServiceName = WorkerData.ServiceName,
                                                 Session = Session,
                                                 Version = WorkerData.Version,
                                                 ContextHttpData = WorkerData.ContextHttpData,
                                                 ReportUpdateIntervalInSeconds = WorkerData.ReportUpdateIntervalInSeconds,
                                             };

            if (mode != ExecutionMode.UpdateAdministrationServer)
            {
                workerDataSessionAndApiKey.DllPath = Path.GetFileName(WorkerData.DllPath);
            }

            switch (mode)
            {
                case ExecutionMode.AdministrationServer:
                    return "--admin \"" + workerDataSessionAndApiKey.SerializeToJsonForCommandPrompt() + "\"";
                case ExecutionMode.UpdateAdministrationServer:
                    return "--update \"" + workerDataSessionAndApiKey.SerializeToJsonForCommandPrompt() + "\"";
                case ExecutionMode.Worker:
                    return "--execute \"" + workerDataSessionAndApiKey.SerializeToJsonForCommandPrompt() + "\"";
                case ExecutionMode.StartWorkerFromAdmin:
                    return "--start \"" + workerDataSessionAndApiKey.SerializeToJsonForCommandPrompt() + "\"";
            }

            throw new NotSupportedException();
        }

        public void Dispose()
        {
            if (workUnitContext != null)
            {
                workUnitContext.Dispose();
                workUnitContext = null;
            }

            if (adminContextServer != null)
            {
                adminContextServer.Dispose();
                adminContextServer = null;
            }

            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
        }

        public void SetUpPermissionsForMonoOnFolder(string folder)
        {
            if (!AbstractChain.IsMono)
            {
                return;
            }

            try
            {
                var chmodProcess = Process.Start(
                    new ProcessStartInfo
                    {
                        FileName = "chmod",
                        Arguments = string.Format("u+x {0}", "*"),
                        WorkingDirectory = folder,
                        UseShellExecute = true
                    });

                chmodProcess.WaitForExit();
            }
            catch (Exception)
            {
            }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            new EmergencyLogger().SaveToFile(exception);

            try
            {
                if (workUnitContext != null)
                {
                    workUnitContext.LogException(exception);
                    workUnitContext.ReportToAdmin();
                }
            }
            catch
            {
            }
        }

        private void CurrentDomain_DomainUnload(object sender, EventArgs e)
        {
            Dispose();
        }

        private static void CopyFiles(string SourcePath, string DestinationPath)
        {
            if (SourcePath[SourcePath.Length - 1] != Path.DirectorySeparatorChar)
            {
                SourcePath += Path.DirectorySeparatorChar;
            }

            if (DestinationPath[DestinationPath.Length - 1] != Path.DirectorySeparatorChar)
            {
                DestinationPath += Path.DirectorySeparatorChar;
            }

            if (!Directory.Exists(SourcePath))
            {
                throw new DirectoryNotFoundException(
                    "Source directory '" + SourcePath + "' does not exists");
            }

            var filesToDelete = Directory.GetFiles(DestinationPath, "*", SearchOption.TopDirectoryOnly);

            foreach (var file in filesToDelete)
            {
                File.Delete(file);
            }

            var allDirectories = Directory.GetDirectories(SourcePath, "*", SearchOption.AllDirectories);
            foreach (string dirPath in allDirectories)
            {
                Directory.CreateDirectory(dirPath.Replace(SourcePath, DestinationPath));
            }

            var allFiles = Directory.GetFiles(SourcePath, "*.*", SearchOption.AllDirectories);
            foreach (string file in allFiles)
            {
                var newFile = file.Replace(SourcePath, DestinationPath);

                if (File.Exists(newFile))
                {
                    File.Delete(newFile);
                }

                File.Copy(file, newFile);
            }
        }

        private object CreateHostedContext(params object [] injectedObjects)
        {
            var contextObject = ExecutionChain.CreateObjectWithParametersAndInjection(WorkerData.ContextType, WorkerData.Parameters, injectedObjects);

            ConnectModules(
                contextObject,
                new object[]
                {
                    workUnitContext, contextObject
                },
                WorkerData.Modules);

            return contextObject;
        }

        private void ConnectModules(object contextObject, object[] injectedObjects, IEnumerable<ModuleStartEntry> modules)
        {
            var modular = contextObject as IModular;

            if (modular == null && modules != null)
            {
                throw new InvalidOperationException("You cannot add modules in a non-modular chain.");
            }

            if (modular == null || modules == null)
            {
                return;
            }

            foreach (var moduleEntry in modules)
            {
                if (!string.IsNullOrEmpty(moduleEntry.ModuleDllPath))
                {
                    LoadAssemblyFromFilePath(moduleEntry.ModuleDllPath);
                }

                var module =
                    ExecutionChain.CreateObjectWithParametersAndInjection(
                        moduleEntry.ModuleType, moduleEntry.ModuleParameters, injectedObjects) as AbstractChain;

                if (module == null)
                {
                    throw new InvalidDataException("Only types derived from AbstractChain are allowed as modules.");
                }

                var optionalModule = module as IOptionalModule;

                if (optionalModule != null)
                {
                    optionalModule.IsEnabled = moduleEntry.IsEnabled;
                }

                if (module is IModular && moduleEntry.Modules != null)
                {
                    var newInjectedParameters = new List<object>(injectedObjects);
                    newInjectedParameters.Add(modular);
                    ConnectModules(module, newInjectedParameters.ToArray(), moduleEntry.Modules);
                    newInjectedParameters.RemoveAt(newInjectedParameters.Count - 1);
                }

                modular.Modules.Add(module);
            }
        }

        private void LoadAssemblyFromFilePath(string assemblyPath)
        {
            assemblyPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assemblyPath));

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            if (assemblies.All(x => x.GetName().Name != Path.GetFileNameWithoutExtension(assemblyPath)))
            {
                Assembly.LoadFile(assemblyPath);
            }
        }
    }
}
