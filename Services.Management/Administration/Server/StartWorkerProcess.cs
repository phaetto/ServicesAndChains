namespace Services.Management.Administration.Server
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using Chains;
    using Chains.Play;
    using Chains.Play.Security;
    using Services.Management.Administration.Executioner;
    using Services.Management.Administration.Worker;

    public sealed class StartWorkerProcess : ReproducibleWithData<StartWorkerData>,
        IChainableAction<AdministrationContext, AdministrationContext>,
        IAuthorizableAction, IApplicationAuthorizableAction
    {
        public StartWorkerProcess(StartWorkerData data)
            : base(data)
        {
        }

        public AdministrationContext Act(AdministrationContext context)
        {
            try
            {
                Data = context.DoRemotable(new PrepareWorkerProcessFiles(Data));

                var destinationPath = context.ServicesFolder + Data.ServiceName + Path.DirectorySeparatorChar
                    + Data.Version + Path.DirectorySeparatorChar + Data.Id + Path.DirectorySeparatorChar;

                var workerExecutioner = new WorkerExecutioner(ExecutionMode.Worker, Data, Session, ApiKey);
                workerExecutioner.SetUpPermissionsForMonoOnFolder(destinationPath);
                var processName = context.HostProcessName.Replace(".vshost", string.Empty);

                Process.Start(
                    new ProcessStartInfo
                    {
                        FileName = destinationPath + processName,
                        Arguments = workerExecutioner.GetProcessArguments(ExecutionMode.Worker),
                        WorkingDirectory = destinationPath,
                        UseShellExecute = true,
                        CreateNoWindow = true,
                        RedirectStandardInput = false,
                        RedirectStandardOutput = false,
                        RedirectStandardError = false,
                    });
            }
            catch (InvalidOperationException ex)
            {
                // When the id already exists
                context.LogException(ex);
            }
            catch (Exception ex)
            {
                context.Do(
                    new StartWorkerProcessWithDelay(
                        new WorkerDataWithDelay
                        {
                            DelayInSeconds = 10,
                            WorkerData = Data
                        }));

                context.LogException(ex);
            }

            return context;
        }

        public string Session { get; set; }

        public string ApiKey { get; set; }
    }
}
