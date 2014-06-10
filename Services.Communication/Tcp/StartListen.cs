namespace Services.Communication.Tcp
{
    using System;
    using Chains;
    using Chains.Play;
    using Chains.Play.Web;
    using SuperSocket.SocketBase;
    using SuperSocket.SocketBase.Config;
    using SuperSocket.SocketEngine;

    public sealed class StartListen : IChainableAction<ServerHost, ServerConnectionContext>
    {
        private readonly string name;
        private readonly string contextTypeName;
        private readonly object contextObject;
        private readonly Func<ExecutableActionSpecification[], bool> onBeforeExecute;
        private readonly Action<dynamic> onAfterExecute;
        private readonly bool newInstanceForEachRequest;

        public StartListen(string name,
            object contextObject,
            Func<ExecutableActionSpecification[], bool> onBeforeExecute = null,
            Action<dynamic> onAfterExecute = null,
            bool newInstanceForEachRequest = false)
        {
            this.name = name;
            this.contextObject = contextObject;
            this.onBeforeExecute = onBeforeExecute;
            this.onAfterExecute = onAfterExecute;
            this.newInstanceForEachRequest = newInstanceForEachRequest;
        }

        public StartListen(string name,
            string contextTypeName,
            Func<ExecutableActionSpecification[], bool> onBeforeExecute = null,
            Action<dynamic> onAfterExecute = null,
            bool newInstanceForEachRequest = false)
        {
            this.name = name;
            this.contextTypeName = contextTypeName;
            this.onBeforeExecute = onBeforeExecute;
            this.onAfterExecute = onAfterExecute;
            this.newInstanceForEachRequest = newInstanceForEachRequest;
        }

        public ServerConnectionContext Act(ServerHost context)
        {
            ServerConnectionContext openedConnection;

            if (contextObject == null)
            {
                openedConnection = new ServerConnectionContext(
                    contextTypeName, context, onBeforeExecute, onAfterExecute, newInstanceForEachRequest);
            }
            else
            {
                openedConnection = new ServerConnectionContext(
                    contextObject, context, onBeforeExecute, onAfterExecute, newInstanceForEachRequest);
            }

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
                                   KeepAliveInterval =  10,
                                   KeepAliveTime = 60,
                               };

            if (!openedConnection.AppServer.Setup(new RootConfig(), serverConfig, new SocketServerFactory()))
            {
                throw new InvalidOperationException("The service '" + name + "' has invalid setup.");
            }

            if (!openedConnection.AppServer.Start())
            {
                throw new InvalidOperationException("The service '" + name + "' could not open. State:" + openedConnection.AppServer.State.ToString());
            }

            return openedConnection;
        }
    }
}
