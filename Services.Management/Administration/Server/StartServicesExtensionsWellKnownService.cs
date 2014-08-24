namespace Services.Management.Administration.Server
{
    using Chains;
    using Services.Management.Administration.Worker;

    public sealed class StartServicesExtensionsWellKnownService : IChainableAction<AdministrationContext, AdministrationContext>
    {
        private const string ContextType = "Services.Extensions.Contexts.ServiceStarter.ServiceStarterContext";
        private const string Id = "auto-service-starter";
        private const string ServiceName = "Services.Extensions";
        private const string DllName = "Services.Extensions.dll";

        public AdministrationContext Act(AdministrationContext context)
        {
            var repoServices = context.DoRemotable(new GetAllRepoServices());

            if (repoServices.RepoServices.ContainsKey(ServiceName))
            {
                context.Do(
                    new StartWorkerProcess(
                        new StartWorkerData
                        {
                            AdminHost = context.AdminServer.Parent.Parent.Hostname,
                            AdminPort = context.AdminServer.Parent.Parent.Port,
                            ContextType = ContextType,
                            HostProcessFileName = context.HostProcessName,
                            Id = Id,
                            ServiceName = ServiceName,
                            DllPath = DllName,
                            ReportUpdateIntervalInSeconds = 30,
                        }));
            }

            return context;
        }
    }
}
