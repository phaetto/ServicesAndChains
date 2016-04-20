﻿namespace Services.Communication.Tcp
{
    using Chains.Play.Web;
    using Services.Communication.Protocol;
    using Services.Communication.Tcp.Servers;

    public class TcpServerProtocolStack : IServerProtocolStack
    {
        private readonly AsyncSocketListener asyncSocketListener;

        private ServerHost ServerHost { get; set; }

        public TcpServerProtocolStack(ProtocolServerLogic protocolServerLogic)
        {
            ProtocolServerLogic = protocolServerLogic;
            asyncSocketListener = new AsyncSocketListener(protocolServerLogic);
        }

        public void OpenServerConnection(ServerHost context)
        {
            asyncSocketListener.StartListening(context, 50);
        }

        public void CloseServerConnection()
        {
            asyncSocketListener.StopListening();
        }

        public object ServerProvider => asyncSocketListener;

        public ProtocolServerLogic ProtocolServerLogic { get; }
    }
}
