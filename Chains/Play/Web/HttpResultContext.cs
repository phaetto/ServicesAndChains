namespace Chains.Play.Web
{
    public sealed class HttpResultContext : HttpResultContextBase<HttpResultContext>
    {
       public HttpResultContext(string resultText = "", string contentType = "text/html", int statusCode = 0, string statusText = null)
            : base(resultText, contentType, statusCode, statusText)
        {
        }

        public HttpResultContext(string statusText, int statusCode)
            : base(statusText, statusCode)
        {
        }

        public HttpResultContext(string redirectTo, bool permanentRedirect)
            : base(redirectTo, permanentRedirect)
        {
        }
    }
}
