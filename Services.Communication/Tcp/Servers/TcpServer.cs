namespace Services.Communication.Tcp.Servers
{
    using Services.Communication.Protocol;
    using SuperSocket.SocketBase;

    public class TcpServer : AppServer<TcpSession>
    {
        internal readonly ProtocolServerLogic ProtocolServerLogic;

        public TcpServer(ProtocolServerLogic protocolServerLogic)
        {
            ProtocolServerLogic = protocolServerLogic;
        }
    }
}
