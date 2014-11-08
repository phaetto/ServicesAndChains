namespace Services.Communication.Protocol
{
    using System;
    using Chains;
    using Chains.Play.Web;

    public sealed class ServerConnectionContext : ChainWithParent<ServerConnectionContext, ServerHost>, IDisposable
    {
        private readonly IServerProtocolStack serverProtocolStack;

        public ServerConnectionContext(ServerHost parent,
            IServerProtocolStack serverProtocolStack)
            : base(parent)
        {
            this.serverProtocolStack = serverProtocolStack;
            serverProtocolStack.OpenServerConnection(parent);
        }

        public void Close()
        {
            serverProtocolStack.CloseServerConnection();
        }

        public void Dispose()
        {
            Close();
        }

        public T GetServerProvider<T>() where T : class
        {
            return serverProtocolStack.ServerProvider as T;
        }
    }
}
