﻿namespace Services.Communication.Tcp
{
    using System;
    using Chains.Play.Web;
    using Services.Communication.Protocol;
    using Services.Communication.Tcp.Servers;
    using SuperSocket.SocketBase;
    using SuperSocket.SocketBase.Config;

    public class TcpServerProtocolStack : IServerProtocolStack
    {
        private TcpServer tcpServer;

        public TcpServerProtocolStack(ProtocolServerLogic protocolServerLogic)
        {
            ProtocolServerLogic = protocolServerLogic;
            tcpServer = new TcpServer(protocolServerLogic);
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

            if (!tcpServer.Setup(new RootConfig(), serverConfig))
            {
                throw new InvalidOperationException("The service '" + name + "' has invalid setup.");
            }

            if (!tcpServer.Start())
            {
                throw new InvalidOperationException("The service '" + name + "' could not open. State:" + tcpServer.State.ToString());
            }
        }

        public void CloseServerConnection()
        {
            if (tcpServer == null)
            {
                return;
            }

            try
            {
                tcpServer.Stop();
                tcpServer.Dispose();
            }
            finally
            {
                tcpServer = null;
            }
        }

        public object ServerProvider
        {
            get
            {
                return tcpServer;
            }
        }

        public ProtocolServerLogic ProtocolServerLogic { get; private set; }
    }
}
