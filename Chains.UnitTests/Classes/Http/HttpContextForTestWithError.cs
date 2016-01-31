namespace Chains.UnitTests.Classes.Http
{
    using System;
    using System.Net;
    using Chains.Play.Web.HttpListener;

    public class HttpContextForTestWithError : Chain<HttpContextForTestWithError>, IHttpRequestHandler
    {
        public readonly string Output;

        public HttpContextForTestWithError(string output)
        {
            this.Output = output;
        }

        public bool ResolveRequest(HttpListenerContext context)
        {
            throw new InvalidOperationException("The HttpContextForTestWithError failed.");
        }
    }
}
