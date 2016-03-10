namespace Services.Management.Administration.Update
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Chains;
    using Chains.Play;
    using Chains.Play.Security;
    using Services.Management.Administration.Executioner;
    using Services.Management.Administration.Server;
    using Services.Management.Administration.Worker;

    public sealed class ApplyAdminUpdate : ReproducibleWithSerializableData<string>,
        IChainableAction<AdministrationContext, AdministrationContext>,
        IAuthorizableAction, IApplicationAuthorizableAction
    {
        public ApplyAdminUpdate(string updateFolder)
            : base(updateFolder)
        {
        }

        public AdministrationContext Act(AdministrationContext context)
        {
            if (Data.Last() == Path.DirectorySeparatorChar)
            {
                Data = Data.Substring(0, Data.Length - 1);
            }

            var processName = context.HostProcessName.Replace(".vshost", string.Empty);

            if (context.ReportData.Any(x => x.Value.AdviceGiven == AdviceState.Restart))
            {
                throw new InvalidOperationException("Cannot update now, the application has work units at restart state.");
            }

            var workerExecutioner = new WorkerExecutioner(
                ExecutionMode.UpdateAdministrationServer,
                new StartWorkerData
                {
                    AdminHost = context.Parent.Parent.Hostname,
                    AdminPort = context.Parent.Parent.Port,
                    DllPath = AppDomain.CurrentDomain.BaseDirectory + processName,
                    HostProcessFileName = context.HostProcessName
                });

            workerExecutioner.SetUpPermissionsForMonoOnFolder(Data);

            var fileName = Data + Path.DirectorySeparatorChar + processName;
            var arguments = workerExecutioner.GetProcessArguments(ExecutionMode.UpdateAdministrationServer);
            if (AbstractChain.IsMono)
            {
                arguments = $"\"{fileName}\" {arguments}";
                fileName = "mono";
            }

            Process.Start(
                new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    WorkingDirectory = Data,
                    UseShellExecute = !AbstractChain.IsMono,
                    CreateNoWindow = true,
                    RedirectStandardInput = false,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                });

            return context;
        }

        public string Session { get; set; }

        public string ApiKey { get; set; }
    }
}
