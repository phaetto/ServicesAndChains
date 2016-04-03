namespace Chains.Play.Web.HttpListener.Handlers
{
    using System.Net;
    using Chains;
    using Chains.Play.Web;
    using Chains.Play.Web.HttpListener;

    public sealed class RedirectHttpHandler : Chain<RedirectHttpHandler>, IHttpRequestHandler
    {
        private readonly string redirectAddressTo;

        public RedirectHttpHandler(HttpServer httpServer, string redirectAddressTo, string path = "/")
        {
            this.redirectAddressTo = redirectAddressTo;
            httpServer.Modules.Add(this);
            httpServer.AddPath(path);
        }

        public bool ResolveRequest(HttpListenerContext context)
        {
            new HttpResultContext(redirectAddressTo, false).ApplyOutputToHttpContext(context);

            return true;
        }
    }
}
