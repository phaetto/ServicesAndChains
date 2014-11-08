namespace Services.Communication.Protocol
{
    using System;
    using Chains;
    using Chains.Play;
    using Chains.Play.Web;
    using Services.Communication.Tcp;
    using Services.Communication.Web;

    public sealed class StartListen : IChainableAction<ServerHost, ServerConnectionContext>
    {
        private readonly string contextTypeName;
        private readonly object contextObject;
        private readonly Func<ExecutableActionSpecification[], bool> onBeforeExecute;
        private readonly Action<dynamic> onAfterExecute;
        private readonly bool newInstanceForEachRequest;
        private readonly ProtocolType protocolType;
        private readonly IServerProtocolStack serverProtocolStack;

        public StartListen(object contextObject,
            Func<ExecutableActionSpecification[], bool> onBeforeExecute = null,
            Action<dynamic> onAfterExecute = null,
            bool newInstanceForEachRequest = false,
            ProtocolType protocolType = ProtocolType.Tcp,
            IServerProtocolStack serverProtocolStack = null)
        {
            this.contextObject = contextObject;
            this.onBeforeExecute = onBeforeExecute;
            this.onAfterExecute = onAfterExecute;
            this.newInstanceForEachRequest = newInstanceForEachRequest;
            this.protocolType = protocolType;
            this.serverProtocolStack = serverProtocolStack;
        }

        public StartListen(string contextTypeName,
            Func<ExecutableActionSpecification[], bool> onBeforeExecute = null,
            Action<dynamic> onAfterExecute = null,
            bool newInstanceForEachRequest = false,
            ProtocolType protocolType = ProtocolType.Tcp,
            IServerProtocolStack serverProtocolStack = null)
        {
            this.contextTypeName = contextTypeName;
            this.onBeforeExecute = onBeforeExecute;
            this.onAfterExecute = onAfterExecute;
            this.newInstanceForEachRequest = newInstanceForEachRequest;
            this.protocolType = protocolType;
            this.serverProtocolStack = serverProtocolStack;
        }

        public StartListen(Func<ExecutableActionSpecification[], bool> onBeforeExecute = null,
            Action<dynamic> onAfterExecute = null,
            bool newInstanceForEachRequest = false,
            ProtocolType protocolType = ProtocolType.Tcp,
            IServerProtocolStack serverProtocolStack = null)
        {
            this.onBeforeExecute = onBeforeExecute;
            this.onAfterExecute = onAfterExecute;
            this.newInstanceForEachRequest = newInstanceForEachRequest;
            this.protocolType = protocolType;
            this.serverProtocolStack = serverProtocolStack;
        }

        public ServerConnectionContext Act(ServerHost context)
        {
            ProtocolServerLogic protocolServerLogic;

            if (!string.IsNullOrWhiteSpace(contextTypeName))
            {
                protocolServerLogic = new ProtocolServerLogic(contextTypeName, onBeforeExecute, onAfterExecute, newInstanceForEachRequest);
            }
            else if (contextObject != null)
            {
                protocolServerLogic = new ProtocolServerLogic(contextObject, onBeforeExecute, onAfterExecute, newInstanceForEachRequest);
            }
            else
            {
                protocolServerLogic = new ProtocolServerLogic(onBeforeExecute, onAfterExecute, newInstanceForEachRequest);
            }

            switch (protocolType)
            {
                case ProtocolType.Tcp:
                    return new ServerConnectionContext(context, new TcpServerProtocolStack(protocolServerLogic));
                case ProtocolType.Http:
                    return new ServerConnectionContext(context, new HttpServerProtocolStack(protocolServerLogic));
                case ProtocolType.Custom:
                    if (serverProtocolStack == null)
                    {
                        throw new InvalidOperationException(
                            "No well known protocol picked nor serverProtocolStack has been added");
                    }

                    return new ServerConnectionContext(context, serverProtocolStack);
            }

            throw new NotSupportedException();
        }
    }
}
