namespace Services.Communication.Tcp
{
    using System;
    using System.Net.Sockets;
    using Chains;

    [Obsolete]
    public sealed class RetryConnection : IChainableAction<ClientConnectionContext, ClientConnectionContext>
    {
        public ClientConnectionContext Act(ClientConnectionContext context)
        {
            lock (context)
            {
                try
                {
                    context.Close();
                }
                catch
                {
                }

                context.ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                context.ClientSocket.Connect(context.Parent.Hostname, context.Parent.Port);
            }

            return context;
        }
    }
}
