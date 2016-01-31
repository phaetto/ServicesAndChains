namespace Services.Communication.Protocol
{
    using System;
    using System.Threading;
    using Chains;
    using Chains.Play.Web;

    public sealed class WaitUntilClientConnects : IChainableAction<Client, ClientConnectionContext>
    {
        public readonly int WaitDelayInMilliseconds;

        public WaitUntilClientConnects(int waitDelayInMilliseconds = 5000)
        {
            this.WaitDelayInMilliseconds = waitDelayInMilliseconds;
        }

        public ClientConnectionContext Act(Client context)
        {
            while (true)
            {
                try
                {
                    return context.Do(new OpenConnection());
                }
                catch (Exception)
                {
                }

                Thread.Sleep(WaitDelayInMilliseconds);
            }
        }
    }
}
