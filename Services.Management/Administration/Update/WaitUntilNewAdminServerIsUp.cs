namespace Services.Management.Administration.Update
{
    using System;
    using System.Net.Sockets;
    using System.Threading;
    using Chains;
    using Services.Communication.Protocol;
    using Services.Management.Administration.Server;

    public sealed class WaitUntilNewAdminServerIsUp : IChainableAction<ClientConnectionContext, ClientConnectionContext>
    {
        private readonly DateTime previousServerDateTimeStarted;

        public readonly int WaitDelayInMilliseconds;

        public WaitUntilNewAdminServerIsUp(DateTime previousServerDateTimeStarted, int waitDelayInMilliseconds = 5000)
        {
            this.previousServerDateTimeStarted = previousServerDateTimeStarted;
            this.WaitDelayInMilliseconds = waitDelayInMilliseconds;
        }

        public ClientConnectionContext Act(ClientConnectionContext context)
        {
            while (true)
            {
                try
                {
                    context.Do(new RetryConnection());

                    var newServerDateTimeStarted =
                            context.Do(new Send<AdministrationData>(new GetAdministratorData())).StartedOn;

                    if (previousServerDateTimeStarted < newServerDateTimeStarted)
                    {
                        return context;
                    }
                }
                catch (SocketException)
                {
                }

                Thread.Sleep(WaitDelayInMilliseconds);
            }
        }
    }
}
