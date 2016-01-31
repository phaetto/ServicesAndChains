namespace Services.Management.Administration.Update
{
    using System.Threading;
    using Chains;
    using Chains.Play.Security;
    using Services.Communication.Protocol;
    using Services.Management.Administration.Server;

    public sealed class WaitUntilServiceIsUninstalled : IChainableAction<ClientConnectionContext, ClientConnectionContext>,
        IAuthorizableAction,
        IApplicationAuthorizableAction
    {
        private readonly string serviceName;

        public readonly int WaitDelayInMilliseconds;

        public WaitUntilServiceIsUninstalled(string serviceName, int waitDelayInMilliseconds = 5000)
        {
            this.serviceName = serviceName;
            this.WaitDelayInMilliseconds = waitDelayInMilliseconds;
        }

        public ClientConnectionContext Act(ClientConnectionContext context)
        {
            while (true)
            {
                var reportedData = context.Do(new Send<GetAllRepoServicesReturnData>(new GetAllRepoServices()
                                                                                     {
                                                                                         ApiKey = ApiKey,
                                                                                         Session = Session
                                                                                     }));

                if (!reportedData.RepoServices.ContainsKey(serviceName))
                {
                    return context;
                }

                Thread.Sleep(WaitDelayInMilliseconds);
            }
        }

        public string Session { get; set; }

        public string ApiKey { get; set; }
    }
}
