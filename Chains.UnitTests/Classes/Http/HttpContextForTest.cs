namespace Chains.UnitTests.Classes.Security
{
    using System.Net;
    using Chains.Play.Web;
    using Chains.Play.Web.HttpListener;

    public class HttpContextForTest : Chain<HttpContextForTest>, IHttpRequestHandler
    {
        public readonly string output;

        public HttpContextForTest(string output)
        {
            this.output = output;
        }

        public bool ResolveRequest(HttpListenerContext context)
        {
            new HttpResultContext(output).ApplyOutputToHttpContext(context);

            return true;
        }
    }
}
