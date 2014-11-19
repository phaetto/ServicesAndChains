namespace Services.Communication.Web
{
    using System;
    using Chains.Play.Web;
    using Chains.Play.Web.HttpListener;
    using Services.Communication.Protocol;

    public class HttpServerProtocolStack : IServerProtocolStack
    {
        private readonly ProtocolServerLogic protocolServerLogic;

        private HttpServer httpServer;

        public HttpServerProtocolStack(ProtocolServerLogic protocolServerLogic)
        {
            this.protocolServerLogic = protocolServerLogic;
        }

        public void OpenServerConnection(ServerHost context)
        {
            if (context.Parent.Path == null)
            {
                throw new InvalidOperationException("The http protocol stack needs to include the 'path' in server (non-null).");
            }

            httpServer = context.Do(new StartHttpServer(null, context.ServerThreads));
            httpServer.Modules.Add(new HttpServerRequestHandler(httpServer, protocolServerLogic));
        }

        public void CloseServerConnection()
        {
            httpServer.Stop();
        }

        public object ServerProvider {
            get
            {
                return httpServer;
            }
        }

        public ProtocolServerLogic ProtocolServerLogic
        {
            get
            {
                return protocolServerLogic;
            }
        }
    }
}
