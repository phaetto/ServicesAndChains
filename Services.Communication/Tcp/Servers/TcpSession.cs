namespace Services.Communication.Tcp.Servers
{
    using System;
    using System.Text;
    using SuperSocket.SocketBase;

    public class TcpSession : AppSession<TcpSession>
    {
        public TcpServer Server
        {
            get
            {
                return (TcpServer)this.AppServer;
            }
        }

        public TcpSession()
        {
            this.Charset = Encoding.ASCII;
        }

        protected override void HandleException(Exception ex)
        {
            Send(Server.ProtocolServerLogic.Serialize(ex));
        }
    }
}
