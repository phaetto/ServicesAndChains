namespace Chains.Play.Web
{
    using System;
    using System.IO;
    using System.Collections.Specialized;
    using System.Collections;
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

        public HttpContext HttpContext
        {
            get
            {
                return httpContext;
            }
        }

        public HttpListenerContext HttpListenerContext
        {
            get
            {
                return httpListenerContext;
            }
        }

        public NameValueCollection QueryString
        {
            get
            {
                return httpListenerContext != null
                    ? httpListenerContext.Request.QueryString
                    : (queryStringRequestData ?? httpContext.Request.QueryString);
            }
        }

        public NameValueCollection Form
        {
            get
            {
                // No form for listener - use the imput string and decode only if "application/x-www-form-urlencoded"
                if (httpListenerContext != null
                    && httpListenerContext.Request.ContentType != null
                    && httpListenerContext.Request.ContentType.ToLowerInvariant().StartsWith(HttpRequest.FormUrlEncodedContentType)
                    && formRequestData.Count == 0)
                {
                    var input = string.Empty;
                    using (var streamReader = new StreamReader(httpListenerContext.Request.InputStream))
                    {
                        input = streamReader.ReadToEnd();
                    }

                    var formData = input.Split(
                        new[]
                        {
                            '&'
                        });

                    foreach (var formNameValue in formData)
                    {
                        var nameValue = formNameValue.Split(
                            new[]
                            {
                                '='
                            });

                        formRequestData.Add(nameValue[0], nameValue[1]);
                    }
                }

                return formRequestData ?? httpContext.Request.Form;
            }
        }

        public string this[string name]
        {
            get
            {
                return Form[name] ?? QueryString[name];
            }
        }

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

        public Stream InputStream
        {
            get
            {
                return httpListenerContext != null
                    ? httpListenerContext.Request.InputStream
                    : (httpContext != null ? httpContext.Request.InputStream : inputStream);
            }
        }

        public string HttpMethod
        {
            get
            {
                return httpListenerContext != null
                    ? httpListenerContext.Request.HttpMethod
                    : (httpMethod ?? httpContext.Request.HttpMethod);
            }
        }

        public string RequestPath
        {
            get
            {
                return httpListenerContext != null
                    ? httpListenerContext.Request.Url.AbsolutePath
                    : (requestPath ?? httpContext.Request.Path);
            }
        }

        public string Host
        {
            get
            {
                return httpListenerContext != null
                    ? httpListenerContext.Request.Url.Host
                    : (host ?? httpContext.Request.Url.Host);
            }
        }

        public int Port
        {
            get
            {
                return httpListenerContext != null
                    ? httpListenerContext.Request.Url.Port
                    : (port > 0 ? port : httpContext.Request.Url.Port);
            }
        }

        public bool HasSession()
        {
            return SessionData != null || httpContext.Session != null;
        }

        public object Session(string name)
        {
            return httpContext == null ? SessionData[name] : httpContext.Session[name];
        }

        public void Session(string name, object value)
        {
            if (httpContext != null)
                httpContext.Session[name] = value;
            else
                SessionData[name] = value;
        }

        public object Application(string name)
        {
            return applicationData != null ? applicationData[name] : httpContext.Application[name];
        }

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
                return httpContext.Request.Url.Scheme + "://" + httpContext.Request.Url.Host
                    + (httpContext.Request.Url.Port != 80 ? ":" + httpContext.Request.Url.Port : "");
            }

            if (httpListenerContext != null)
            {
                return httpListenerContext.Request.Url.Scheme + "://" + httpListenerContext.Request.Url.Host
                     + (httpListenerContext.Request.Url.Port != 80 ? ":" + httpListenerContext.Request.Url.Port : "");
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

        private void PrepareBaseFolder(string baseFolder)
        {
            this.baseFolder = baseFolder ?? AppDomain.CurrentDomain.BaseDirectory;
            this.baseFolder = this.baseFolder.EndsWith(Path.DirectorySeparatorChar.ToString())
                ? this.baseFolder.Substring(0, this.baseFolder.Length - 1)
                : this.baseFolder;
        }
    }
}
