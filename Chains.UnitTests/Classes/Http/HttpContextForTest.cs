namespace Chains.UnitTests.Classes.Http
{
    using System.Net;
    using Chains.Play.Web;
    using Chains.Play.Web.HttpListener;

    public class HttpContextForTest : Chain<HttpContextForTest>, IHttpRequestHandler
    {
        public readonly string Output;

        public HttpContextForTest(string output)
        {
            this.Output = output;
        }

        public bool ResolveRequest(HttpListenerContext context)
        {
            new HttpResultContext(Output).ApplyOutputToHttpContext(context);

            return true;
        }
    }
}
