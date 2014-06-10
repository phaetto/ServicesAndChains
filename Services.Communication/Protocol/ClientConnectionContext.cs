namespace Services.Communication.Protocol
{
    using System;
    using Chains;
    using Chains.Play.Web;

    public sealed class ClientConnectionContext : ChainWithParent<ClientConnectionContext, Client>, IDisposable
    {
        public readonly IClientProtocolStack ClientProtocolStack;

        public ClientConnectionContext(Client chain, IClientProtocolStack clientProtocolStack)
            : base(chain)
        {
            ClientProtocolStack = clientProtocolStack;
            ClientProtocolStack.OpenClientConnection(chain);
        }

        public void Close()
        {
            ClientProtocolStack.CloseClientConnection();
        }

        public void Dispose()
        {
            Close();
        }
    }
}
