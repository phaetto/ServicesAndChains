namespace Chains.Play.Web
{
    using System.Collections.Specialized;
    using System.IO;
    using System.Net;
    using System.Web;

    public interface IHttpContextInfo
    {
        HttpContext HttpContext { get; }

        HttpListenerContext HttpListenerContext { get; }

        NameValueCollection QueryString { get; }

        NameValueCollection Form { get; }

        string this[string name] { get; }

        HttpCookieCollection Cookies { get; }

        Stream InputStream { get; }

        string HttpMethod { get; }

        bool HasSession();

        object Session(string name);

        void Session(string name, object value);

        object Application(string name);

        void Application(string name, object value);

        string RequestPath { get; }

        string Host { get; }

        int Port { get; }

        string ServerMapPath(string uri);

        string ServerUri();

        string PageUri();
    }
}
