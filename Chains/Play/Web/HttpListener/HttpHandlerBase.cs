namespace Chains.Play.Web.HttpListener
{
    using System.Net;
    using Chains;

    public abstract class HttpHandlerBase : AbstractChain, IHttpRequestHandler
    {
        protected readonly string RequestPath;

        protected readonly string DefaultDocument;

        protected HttpHandlerBase(HttpServer httpServer, string path, string defaultDocument = null, bool allowToSharePath = false)
        {
            DefaultDocument = defaultDocument;
            RequestPath = path.EndsWith("/") ? path.Substring(0, path.Length - 1) : path;
            httpServer.Modules.Add(this);

            try
            {
                httpServer.AddPath(RequestPath + "/");
            }
            catch (HttpListenerException)
            {
                if (!allowToSharePath)
                {
                    throw;
                }
            }
        }

        protected bool CheckPathForMatch(HttpListenerContext context)
        {
            return context.Request.Url.AbsolutePath.ToLowerInvariant() == RequestPath
                || context.Request.Url.AbsolutePath.ToLowerInvariant() == RequestPath + "/"
                || (!string.IsNullOrWhiteSpace(DefaultDocument) && context.Request.Url.AbsolutePath.ToLowerInvariant() == RequestPath + "/" + DefaultDocument);
        }

        public abstract bool ResolveRequest(HttpListenerContext context);
    }
}
