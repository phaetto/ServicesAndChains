namespace Services.Communication.Tcp
{
    using System;
    using Chains.Play.Web;
    using Services.Communication.Protocol;
    using Services.Communication.Tcp.Servers;
    using SuperSocket.SocketBase;
    using SuperSocket.SocketBase.Config;
    using SuperSocket.SocketEngine;

    public class TcpServerProtocolStack : IServerProtocolStack
    {
        private TcpServer AppServer;

        public TcpServerProtocolStack(ProtocolServerLogic protocolServerLogic)
        {
            AppServer = new TcpServer(protocolServerLogic);
        }

        public void OpenServerConnection(ServerHost context)
        {
            var name = string.Format("{0}:{1}", context.Parent.Hostname, context.Parent.Port);

            var serverConfig = new ServerConfig
            {
                Ip = context.Parent.Hostname,
                Port = context.Parent.Port,
                Name = string.Format("{0}-{1}", name, Guid.NewGuid().ToString()),
                Mode = SocketMode.Tcp,
                MaxConnectionNumber = 5000,
                DisableSessionSnapshot = true,
                ClearIdleSession = false,
                MaxRequestLength = int.MaxValue,
                KeepAliveInterval = 10,
                KeepAliveTime = 60,
            };

            if (!AppServer.Setup(new RootConfig(), serverConfig, new SocketServerFactory()))
            {
                throw new InvalidOperationException("The service '" + name + "' has invalid setup.");
            }

            if (!AppServer.Start())
            {
                throw new InvalidOperationException("The service '" + name + "' could not open. State:" + AppServer.State.ToString());
            }
        }

        public void CloseServerConnection()
        {
            if (AppServer == null)
            {
                return;
            }

            try
            {
                AppServer.Stop();
                AppServer.Dispose();
            }
            finally
            {
                AppServer = null;
            }
        }
    }
}
