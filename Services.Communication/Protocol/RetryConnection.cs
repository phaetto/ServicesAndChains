namespace Services.Communication.Protocol
{
    using System.Net.Sockets;
    using Chains;

    public sealed class RetryConnection : IChainableAction<ClientConnectionContext, ClientConnectionContext>
    {
        public ClientConnectionContext Act(ClientConnectionContext context)
        {
            lock (context)
            {
                context.ClientProtocolStack.RetryClientConnection(context.Parent);
            }

            return context;
        }
    }
}
