namespace Services.Communication.Tcp
{
    using System;
    using Chains;
    using Chains.Play;
    using Chains.Play.Web;
    using Services.Communication.Protocol;
    using Services.Communication.Tcp.Servers;

    public sealed class ServerConnectionContext : ChainWithParent<ServerConnectionContext, ServerHost>, IDisposable
    {
        public readonly ProtocolServerLogic ProtocolServerLogic;
        public TcpServer AppServer { get; private set; }

        public ServerConnectionContext(object contextObject,
            ServerHost parent,
            Func<ExecutableActionSpecification[], bool> onBeforeExecute = null,
            Action<dynamic> onAfterExecute = null,
            bool newInstanceForEachRequest = false)
            : base(parent)
        {
            ProtocolServerLogic = new ProtocolServerLogic(contextObject, onBeforeExecute, onAfterExecute, newInstanceForEachRequest);
            AppServer = new TcpServer(ProtocolServerLogic);
        }

        public ServerConnectionContext(string contextTypeName,
            ServerHost parent,
            Func<ExecutableActionSpecification[], bool> onBeforeExecute = null,
            Action<dynamic> onAfterExecute = null,
            bool newInstanceForEachRequest = false)
            : base(parent)
        {
            ProtocolServerLogic = new ProtocolServerLogic(contextTypeName, onBeforeExecute, onAfterExecute, newInstanceForEachRequest);
            AppServer = new TcpServer(ProtocolServerLogic);
        }

        public void Close()
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

        public void Dispose()
        {
            Close();
        }
    }
}
