namespace Services.Communication.Protocol
{
    using System;
    using Chains;
    using Chains.Play.Web;

    public sealed class ServerConnectionContext : ChainWithParent<ServerConnectionContext, ServerHost>, IDisposable
    {
        public readonly IServerProtocolStack ServerProtocolStack;

        public ServerConnectionContext(ServerHost parent,
            IServerProtocolStack serverProtocolStack)
            : base(parent)
        {
            this.ServerProtocolStack = serverProtocolStack;
            serverProtocolStack.OpenServerConnection(parent);
        }

        public void Close()
        {
            ServerProtocolStack.CloseServerConnection();
        }

        public void Dispose()
        {
            Close();
        }
    }
}
