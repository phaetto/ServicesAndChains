namespace Services.Communication.Tcp
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using Chains;
    using Chains.Play.Web;

    [Obsolete]
    public sealed class OpenConnection : IChainableAction<Client, ClientConnectionContext>
    {
        public ClientConnectionContext Act(Client context)
        {
            var clientConnectionContext = new ClientConnectionContext(context);

            var hostname = context.Hostname == IPAddress.Any.ToString()
                ? IPAddress.Loopback.ToString()
                : context.Hostname;

            clientConnectionContext.ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientConnectionContext.ClientSocket.Connect(hostname, context.Port);

            return clientConnectionContext;
        }
    }
}
