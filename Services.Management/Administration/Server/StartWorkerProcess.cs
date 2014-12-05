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
        private readonly int triesLeft;

        public StartWorkerProcess(StartWorkerData data)
            : this(data, 10)
        {
        }

        public StartWorkerProcess(StartWorkerData data, int triesLeft)
            : base(data)
        {
            this.triesLeft = triesLeft;
        }

        public AdministrationContext Act(AdministrationContext context)
        {
            try
            {
                Data = context.Do(new PrepareWorkerProcessFiles(Data));

                var destinationPath = context.ServicesFolder + Data.ServiceName + Path.DirectorySeparatorChar
                    + Data.Version + Path.DirectorySeparatorChar + Data.Id + Path.DirectorySeparatorChar;

                var workerExecutioner = new WorkerExecutioner(ExecutionMode.Worker, Data, Session, ApiKey);
                workerExecutioner.SetUpPermissionsForMonoOnFolder(destinationPath);
                var processName = context.HostProcessName.Replace(".vshost", string.Empty);

                var fileName = destinationPath + processName;
                var arguments = workerExecutioner.GetProcessArguments(ExecutionMode.Worker);
                if (AbstractChain.IsMono)
                {
                    arguments = string.Format("\"{0}\" {1}", fileName, arguments);
                    fileName = "mono";
                }

                Process.Start(
                    new ProcessStartInfo
                    {
                        FileName = fileName,
                        Arguments = arguments,
                        WorkingDirectory = destinationPath,
                        UseShellExecute = true,
                        CreateNoWindow = true,
                        RedirectStandardInput = false,
                        RedirectStandardOutput = false,
                        RedirectStandardError = false,
                    });
            }
            catch (Exception ex)
            {
                if (triesLeft > 0)
                {
                    context.Do(
                        new StartWorkerProcessWithDelay(
                            new WorkerDataWithDelay
                            {
                                DelayInSeconds = 10,
                                WorkerData = Data,
                                TriesLeft = triesLeft - 1
                            }));
                }

                context.LogException(ex);
            }

            return context;
        }

        public string Session { get; set; }

        public string ApiKey { get; set; }
    }
}
