namespace Services.Management.Administration.Update
{
    using System;
    using System.IO;
    using System.Linq;
    using Chains;
    using Chains.Play;
    using Chains.Play.Security;
    using Services.Management.Administration.Server;
    using Services.Management.Administration.Worker;

    public sealed class UninstallService : ReproducibleWithSerializableData<string>,
        IChainableAction<AdministrationContext, AdministrationContext>,
        IAuthorizableAction, IApplicationAuthorizableAction
    {
        public UninstallService(string data)
            : base(data)
        {
        }

        public AdministrationContext Act(AdministrationContext context)
        {
            var repositoryPath = context.RepositoryFolder + Data + Path.DirectorySeparatorChar;

            if (!Directory.Exists(repositoryPath))
            {
                return context;
            }

            if (context.ReportData.Any(x => x.Value.WorkerState == WorkUnitState.Running && x.Value.StartData.ServiceName == Data))
            {
                throw new InvalidOperationException("There are services running. Stop all services and then uninstall.");
            }

            Directory.Delete(repositoryPath, true);

            return context;
        }

        public string Session { get; set; }

        public string ApiKey { get; set; }
    }
}
