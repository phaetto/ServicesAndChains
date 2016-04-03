namespace Chains.Play.Web.HttpListener
{
    using System;
    using System.Net;

    public abstract class HttpRestHandler : HttpHandlerBase
    {
        protected HttpRestHandler(HttpServer httpServer, string path, string defaultDocument = null, bool allowToSharePath = false)
            : base(httpServer, path, defaultDocument, allowToSharePath)
        {
        }

        public override bool ResolveRequest(HttpListenerContext context)
        {
            try
            {
                if (CheckPathForMatch(context))
                {
                    switch (context.Request.HttpMethod.ToLower())
                    {
                        case "get":
                            Get(context);
                            break;
                        case "post":
                            Post(context);
                            break;
                        case "put":
                            Put(context);
                            break;
                        case "delete":
                            Delete(context);
                            break;
                    }

                    return true;
                }
            }
            catch (NotSupportedException)
            {
            }

            return false;
        }

        public abstract void Get(HttpListenerContext context);
        public abstract void Post(HttpListenerContext context);
        public abstract void Put(HttpListenerContext context);
        public abstract void Delete(HttpListenerContext context);
    }
}
