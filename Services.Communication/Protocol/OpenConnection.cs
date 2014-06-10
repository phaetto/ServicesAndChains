namespace Services.Communication.Protocol
{
    using System;
    using Chains;
    using Chains.Play.Web;
    using Services.Communication.Tcp;
    using Services.Communication.Web;

    public sealed class OpenConnection : IChainableAction<Client, ClientConnectionContext>
    {
        private readonly ProtocolType protocolType;
        private readonly IClientProtocolStack clientProtocolStack;

        public OpenConnection(ProtocolType protocolType = ProtocolType.Tcp, IClientProtocolStack clientProtocolStack = null)
        {
            this.protocolType = protocolType;
            this.clientProtocolStack = clientProtocolStack;
        }

        public ClientConnectionContext Act(Client context)
        {
            switch (protocolType)
            {
                case ProtocolType.Tcp:
                    return new ClientConnectionContext(context, new TcpClientProtocolStack());
                case ProtocolType.Http:
                    return new ClientConnectionContext(context, new HttpClientProtocolStack());
                case ProtocolType.Custom:
                    if (clientProtocolStack == null)
                    {
                        throw new InvalidOperationException(
                            "No well known protocol picked nor serverProtocolStack has been added");
                    }

                    return new ClientConnectionContext(context, clientProtocolStack);
            }

            throw new NotSupportedException();
        }
    }
}
