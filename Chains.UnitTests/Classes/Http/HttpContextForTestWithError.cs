namespace Chains.UnitTests.Classes.Security
{
    using System;
    using System.Net;
    using Chains.Play.Web;
    using Chains.Play.Web.HttpListener;

    public class HttpContextForTestWithError : Chain<HttpContextForTestWithError>, IHttpRequestHandler
    {
        public readonly string output;

        public HttpContextForTestWithError(string output)
        {
            this.output = output;
        }

        public bool ResolveRequest(HttpListenerContext context)
        {
            throw new InvalidOperationException("The HttpContextForTestWithError failed.");
        }
    }
}
