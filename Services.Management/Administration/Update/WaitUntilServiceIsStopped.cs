namespace Services.Management.Administration.Update
{
    using System.Threading;
    using Chains;
    using Chains.Play.Security;
    using Services.Communication.Protocol;
    using Services.Management.Administration.Server;
    using Services.Management.Administration.Worker;

    public sealed class WaitUntilServiceIsStopped : IChainableAction<ClientConnectionContext, ClientConnectionContext>,
        IAuthorizableAction,
        IApplicationAuthorizableAction
    {
        private readonly string serviceId;

        public readonly int waitDelayInMilliseconds;

        public WaitUntilServiceIsStopped(string serviceId, int waitDelayInMilliseconds = 5000)
        {
            this.serviceId = serviceId;
            this.waitDelayInMilliseconds = waitDelayInMilliseconds;
        }

        public ClientConnectionContext Act(ClientConnectionContext context)
        {
            while (true)
            {
                var reportedData = context.Do(new Send<GetReportedDataReturnData>(new GetReportedData()
                                                                                  {
                                                                                      ApiKey = ApiKey,
                                                                                      Session = Session
                                                                                  }));

                if (reportedData.Reports[serviceId].WorkerState == WorkUnitState.Stopping)
                {
                    return context;
                }

                Thread.Sleep(waitDelayInMilliseconds);
            }
        }

        public string Session { get; set; }

        public string ApiKey { get; set; }
    }
}
