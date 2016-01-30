namespace Chains.UnitTests.Classes.Http
{
    using System.Net;
    using Chains.Play.Web.HttpListener;

    public class HttpContextForTestNotSupportAnything : Chain<HttpContextForTestNotSupportAnything>, IHttpRequestHandler
    {
        public readonly string output;

        public HttpContextForTestNotSupportAnything(string output)
        {
            this.output = output;
        }

        public bool ResolveRequest(HttpListenerContext context)
        {
            return false;
        }
    }
}
