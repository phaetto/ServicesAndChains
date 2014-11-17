namespace Services.Communication.Tcp.Servers
{
    using System;
    using Services.Communication.Protocol;
    using SuperSocket.SocketBase;

    public class TcpServer : AppServer<TcpSession>, IDisposable
    {
        internal readonly ProtocolServerLogic ProtocolServerLogic;

        public TcpServer(ProtocolServerLogic protocolServerLogic)
        {
            ProtocolServerLogic = protocolServerLogic;
        }

        public void Dispose()
        {
            base.Dispose();
        }
    }
}
