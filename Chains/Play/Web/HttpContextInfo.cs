namespace Chains.Play.Web
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.IO;
    using System.Net;
    using System.Web;

    public sealed class HttpContextInfo : Chain<HttpContextInfo>, IHttpContextInfo
    {
        private readonly HttpContext httpContext;
        private NameValueCollection queryStringRequestData;
        private NameValueCollection formRequestData;
        private static readonly Hashtable SessionData;
        private Hashtable applicationData;
        private HttpCookieCollection cookies;
        private string httpMethod;
        private string requestPath;
        private string host;
        private int port;
        private string baseFolder;
        private Stream inputStream;
        private readonly HttpListenerContext httpListenerContext;

        public HttpContext HttpContext => httpContext;

        public HttpListenerContext HttpListenerContext => httpListenerContext;

        public NameValueCollection QueryString => httpListenerContext?.Request.QueryString ?? (queryStringRequestData ?? httpContext.Request.QueryString);

        public NameValueCollection Form
        {
            get
            {
                // No form for listener - use the imput string and decode only if "application/x-www-form-urlencoded"
                if (httpListenerContext?.Request.ContentType != null
                    && httpListenerContext.Request.ContentType.ToLowerInvariant().StartsWith(HttpRequest.FormUrlEncodedContentType)
                    && formRequestData.Count == 0)
                {
                    var input = string.Empty;
                    using (var streamReader = new StreamReader(httpListenerContext.Request.InputStream))
                    {
                        input = streamReader.ReadToEnd();
                    }

                    var formData = input.Split('&');

                    foreach (var formNameValue in formData)
                    {
                        var nameValue = formNameValue.Split('=');

                        formRequestData.Add(nameValue[0], nameValue[1]);
                    }
                }

                return formRequestData ?? httpContext.Request.Form;
            }
        }

        public string this[string name] => Form[name] ?? QueryString[name];

        public HttpCookieCollection Cookies
        {
            get
            {
                if (httpListenerContext != null && cookies.Count == 0)
                {
                    foreach (Cookie cookie in httpListenerContext.Request.Cookies)
                    {
                        var httpCookie = new HttpCookie(cookie.Name)
                                         {
                                             Value = cookie.Value,
                                             Expires = cookie.Expires,
                                             Domain = cookie.Domain,
                                             HttpOnly = cookie.HttpOnly,
                                             Path = cookie.Path,
                                             Secure = cookie.Secure
                                         };
                        cookies.Add(httpCookie);
                    }
                }

                return cookies ?? httpContext.Request.Cookies;
            }
        }

        public Stream InputStream => httpListenerContext != null
            ? httpListenerContext.Request.InputStream
            : (httpContext?.Request.InputStream ?? inputStream);

        public string HttpMethod => httpListenerContext != null
            ? httpListenerContext.Request.HttpMethod
            : (httpMethod ?? httpContext.Request.HttpMethod);

        public string RequestPath => httpListenerContext?.Request.Url.AbsolutePath ?? (requestPath ?? httpContext.Request.Path);

        public string Host => httpListenerContext?.Request.Url.Host ?? (host ?? httpContext.Request.Url.Host);

        public int Port => httpListenerContext?.Request.Url.Port ?? (port > 0 ? port : httpContext.Request.Url.Port);

        public bool HasSession() => SessionData != null || httpContext.Session != null;

        public object Session(string name) => httpContext == null ? SessionData[name] : httpContext.Session[name];

        public void Session(string name, object value)
        {
            if (httpContext != null)
            {
                httpContext.Session[name] = value;
            }
            else
            {
                SessionData[name] = value;
            }
        }

        public object Application(string name) => applicationData != null ? applicationData[name] : httpContext.Application[name];

        public void Application(string name, object value)
        {
            if (applicationData != null)
                applicationData[name] = value;
            else
                httpContext.Application[name] = value;
        }

        public string ServerMapPath(string uri)
        {
            return httpContext != null
                ? httpContext.Server.MapPath(uri)
                : Path.GetFullPath(
                    Path.Combine(
                        baseFolder,
                        uri.Replace('/', Path.DirectorySeparatorChar).Replace("~", string.Empty).Substring(1)));
        }

        public string ServerUri()
        {
            if (httpContext != null)
            {
                return string.Format(
                    "{0}://{1}{2}",
                    httpContext.Request.Url.Scheme,
                    httpContext.Request.Url.Host,
                    (httpContext.Request.Url.Port != 80 ? ":" + httpContext.Request.Url.Port : ""));
            }

            if (httpListenerContext != null)
            {
                return string.Format(
                    "{0}://{1}{2}",
                    httpListenerContext.Request.Url.Scheme,
                    httpListenerContext.Request.Url.Host,
                    (httpListenerContext.Request.Url.Port != 80 ? ":" + httpListenerContext.Request.Url.Port : ""));
            }

            return "http://" + host + (port != 80 ? ":" + port : "");
        }

        public string PageUri()
        {
            if (httpContext != null)
            {
                return ServerUri() + httpContext.Request.Url.AbsolutePath;
            }

            if (httpListenerContext != null)
            {
                return ServerUri() + httpListenerContext.Request.Url.AbsolutePath;
            }

            return ServerUri() + requestPath;
        }

        static HttpContextInfo()
        {
            SessionData = new Hashtable();
        }

        public HttpContextInfo(HttpContext httpContext)
        {
            this.httpContext = httpContext;
        }

        public HttpContextInfo(HttpListenerContext httpListenerContext)
        {
            this.httpListenerContext = httpListenerContext;
            cookies = new HttpCookieCollection();
            applicationData = new Hashtable();
            formRequestData = new NameValueCollection();

            PrepareBaseFolder(baseFolder);
        }

        public HttpContextInfo(
            string httpMethod = "get",
            string requestPath = "/",
            string host = "nowhere.com",
            int port = 80,
            string baseFolder = null,
            Stream inputStream = null)
        {
            this.httpMethod = httpMethod;
            this.requestPath = requestPath;
            this.host = host;
            this.port = port;

            PrepareBaseFolder(baseFolder);

            queryStringRequestData = new NameValueCollection();
            formRequestData = new NameValueCollection();
            applicationData = new Hashtable();
            cookies = new HttpCookieCollection();
            this.inputStream = inputStream ?? new MemoryStream();
        }

        private void PrepareBaseFolder(string baseFolderToPrepare)
        {
            baseFolder = baseFolderToPrepare ?? AppDomain.CurrentDomain.BaseDirectory;
            baseFolder = baseFolder.EndsWith(Path.DirectorySeparatorChar.ToString())
                ? baseFolder.Substring(0, baseFolder.Length - 1)
                : baseFolder;
        }
    }
}
