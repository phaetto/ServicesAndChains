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

            if (!pathLowerInvariant.EndsWith("/"))
            {
                pathLowerInvariant += "/";
            }

            httpServer.AddPath(pathLowerInvariant);

            var escapedPath = Uri.EscapeUriString(pathLowerInvariant);

            paths.Add(escapedPath);
        }

        public bool ResolveRequest(HttpListenerContext context)
        {
            var absolutePath = context.Request.Url.AbsolutePath;

            if (!absolutePath.EndsWith("/"))
            {
                absolutePath += "/";
            }

            if (paths.Contains(absolutePath) && context.Request.HttpMethod.ToLowerInvariant() == "post")
            {
                var httpInfoContext = new HttpContextInfo(context);

                string postJson;
                using (var streamReader = new StreamReader(httpInfoContext.InputStream))
                {
                    postJson = streamReader.ReadToEnd();
                }

                if (string.IsNullOrWhiteSpace(postJson))
                {
                    throw new InvalidOperationException("Post data are empty, this is not allowed.");
                }

                var resultString = protocolServerLogic.ReadFromStreamAndPlay(postJson);

                if (context.Request.Headers.Get(HttpClientProtocolStack.HeaderKeyAndValue) != HttpClientProtocolStack.HeaderKeyAndValue)
                {
                    new HttpResultContext(resultString, "text/json")
                        .AccessControlAllowOriginAll()
                        .CompressRequest()
                        .ApplyOutputToHttpContext(httpInfoContext);
                }
                else
                {
                    HttpResultContext.NoContent().ApplyOutputToHttpContext(httpInfoContext);
                }

                return true;
            }

            return false;
        }
    }
}
