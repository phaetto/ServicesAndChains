namespace Services.Management.Administration.Worker
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using Chains;
    using Chains.Play.Web;
    using Services.Communication.Protocol;

    public sealed class StartWorkUnit : IChainableAction<WorkUnitContext, WorkUnitContext>
    {
        public WorkUnitContext Act(WorkUnitContext context)
        {
            if (!string.IsNullOrEmpty(context.WorkerData.DllPath))
            {
                context.WorkerData.DllPath = CalculateAssemblyPath(context.WorkerData.DllPath);

                if (File.Exists(context.WorkerData.DllPath + ".config"))
                {
                    var processConfig = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar
                        + context.WorkerData.HostProcessFileName + ".config";

                    if (File.Exists(processConfig))
                    {
                        File.Delete(processConfig);
                    }

                    File.Copy(context.WorkerData.DllPath + ".config", processConfig);
                }

                LoadAssembly(context.WorkerData.DllPath);
            }

            context.AdminServer = new Client(context.WorkerData.AdminHost, context.WorkerData.AdminPort).Do(new OpenConnection());

            context.State = WorkUnitState.Running;

            context.TimeStarted = DateTime.UtcNow;

            context.ReportThread = new Thread(context.ReportToAdminThread);
            context.ReportThread.Start();

            return context;
        }

        public static void LoadAssemblyFromFilePath(string assemblyPath)
        {
            assemblyPath = CalculateAssemblyPath(assemblyPath);

            LoadAssembly(assemblyPath);
        }

        private static string CalculateAssemblyPath(string assemblyPath)
        {
            assemblyPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assemblyPath));
            return assemblyPath;
        }

        private static void LoadAssembly(string assemblyPath)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            if (assemblies.All(x => x.GetName().Name != Path.GetFileNameWithoutExtension(assemblyPath)))
            {
                Assembly.LoadFile(assemblyPath);
            }
        }
    }
}
