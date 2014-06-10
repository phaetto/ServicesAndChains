namespace Services.Communication.Web
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using Chains;
    using Chains.Play.Web;
    using Chains.Play.Web.HttpListener;
    using Services.Communication.Protocol;

    internal class HttpServerRequestHandler : Chain<HttpServerRequestHandler>, IHttpRequestHandler
    {
        private readonly ProtocolServerLogic protocolServerLogic;

        private readonly List<string> paths = new List<string>();

        public HttpServerRequestHandler(HttpServer httpServer, ProtocolServerLogic protocolServerLogic)
        {
            this.protocolServerLogic = protocolServerLogic;

            var pathLowerInvariant = httpServer.Parent.Parent.Path.ToLowerInvariant();

            httpServer.AddPath(pathLowerInvariant + "/send/");
            httpServer.AddPath(pathLowerInvariant + "/send-receive/");

            var escapedPath = Uri.EscapeUriString(pathLowerInvariant);

            paths.Add(escapedPath + "/send/");
            paths.Add(escapedPath + "/send-receive/");
            paths.Add(escapedPath + "/send");
            paths.Add(escapedPath + "/send-receive");
        }

        public bool ResolveRequest(HttpListenerContext context)
        {
            if (paths.Contains(context.Request.Url.AbsolutePath) && context.Request.HttpMethod.ToLowerInvariant() == "post")
            {
                var httpInfoContext = new HttpContextInfo(context);

                var postJson = string.Empty;
                using (var streamReader = new StreamReader(httpInfoContext.InputStream))
                {
                    postJson = streamReader.ReadToEnd();
                }

                if (string.IsNullOrWhiteSpace(postJson))
                {
                    throw new InvalidOperationException("Post data are empty, this is not allowed.");
                }

                new HttpResultContext(
                    protocolServerLogic.ApplyDataAndReturn(protocolServerLogic.Deserialize(postJson)), "text/json")
                    .AccessControlAllowOriginAll().CompressRequest().ApplyOutputToHttpContext(httpInfoContext);

                return true;
            }

            return false;
        }
    }
}
