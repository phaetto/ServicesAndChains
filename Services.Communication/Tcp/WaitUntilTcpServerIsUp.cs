namespace Services.Communication.Tcp
{
    using System;
    using System.Threading;
    using Chains;

    public sealed class WaitUntilTcpServerIsUp : IChainableAction<ClientConnectionContext, ClientConnectionContext>
    {
        public readonly int waitDelayInMilliseconds;

        public WaitUntilTcpServerIsUp(int waitDelayInMilliseconds = 5000)
        {
            this.waitDelayInMilliseconds = waitDelayInMilliseconds;
        }

        public ClientConnectionContext Act(ClientConnectionContext context)
        {
            while (true)
            {
                try
                {
                    context.Do(new RetryConnection());

                    return context;
                }
                catch (Exception)
                {
                }

                Thread.Sleep(waitDelayInMilliseconds);
            }
        }
    }
}
