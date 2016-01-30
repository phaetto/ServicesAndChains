namespace Chains.UnitTests.Classes.Http.Security
{
    using System.Net;
    using Chains.Play.Web;
    using Chains.Play.Web.HttpListener;

    public sealed class HttpLoginProviderForTest : Chain<HttpLoginProviderForTest>, IHttpRequestHandler
    {
        public const string UserName = "user-name";

        public const string Password = "1234";

        public readonly string output;

        public HttpLoginProviderForTest(string output, HttpServer server)
        {
            this.output = output;

            server.AddPath("/login/");
        }

        public bool ResolveRequest(HttpListenerContext context)
        {
            if (context.Request.Url.LocalPath == "/login" && context.Request.HttpMethod == "POST")
            {
                var contextInfo = new HttpContextInfo(context);

                if (contextInfo.Form["username"] == UserName && contextInfo.Form["password"] == Password)
                {
                    new HttpResultContext(output).ApplyOutputToHttpContext(contextInfo);

                    return true;
                }
            }

            return false;
        }
    }
}
