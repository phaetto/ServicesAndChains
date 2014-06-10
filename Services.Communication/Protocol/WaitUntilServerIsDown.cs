namespace Services.Communication.Protocol
{
    using System;
    using System.Net.Sockets;
    using System.Threading;
    using Chains;

    public sealed class WaitUntilServerIsDown : IChainableAction<ClientConnectionContext, ClientConnectionContext>
    {
        public readonly int waitDelayInMilliseconds;

        public WaitUntilServerIsDown(int waitDelayInMilliseconds = 5000)
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
                }
                catch (SocketException)
                {
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
