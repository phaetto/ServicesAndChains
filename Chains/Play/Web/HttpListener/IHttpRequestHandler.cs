namespace Chains.Play.Web.HttpListener
{
    using System.Net;

    public interface IHttpRequestHandler
    {
        bool ResolveRequest(HttpListenerContext context);
    }
}
