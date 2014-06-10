namespace Services.Communication.Tcp
{
    using System;
    using System.Threading;
    using Chains;
    using Chains.Play.Web;

    public sealed class WaitUntilClientConnects : IChainableAction<Client, ClientConnectionContext>
    {
        public readonly int waitDelayInMilliseconds;

        public WaitUntilClientConnects(int waitDelayInMilliseconds = 5000)
        {
            this.waitDelayInMilliseconds = waitDelayInMilliseconds;
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

                Thread.Sleep(waitDelayInMilliseconds);
            }
        }
    }
}
