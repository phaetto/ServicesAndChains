namespace Chains.Play.Web
{
    using System;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;
    using System.Net;
    using System.Text;
    using System.Web;

    public class HttpResultContextBase<T> : Chain<T>, IAggreggatable<T>
        where T : HttpResultContextBase<T>
    {
        private const string Html404DefaultBody = "<html><body><h1>{0}</h1><p>{1}</p></body></html>";

        public readonly int StatusCode;
        public readonly string StatusText;
        public readonly NameValueCollection ResponseHeaders = new NameValueCollection();
        public readonly HttpCookieCollection ResponseCookies = new HttpCookieCollection();
        public readonly bool PermanentRedirect;
        public readonly StringBuilder ResponseText = new StringBuilder();

        public DateTime WhenModified { get; set; }
        public bool IsCompressing { get; set; }
        public bool IsNotCached { get; set; }
        public string RedirectTo { get; set; }
        public string ContentType { get; set; }
        public TimeSpan MaxAge { get; set; }
        public string FileName { get; set; }

        public HttpResultContextBase(string resultText = "", string contentType = "text/html", int statusCode = 0, string statusText = null)
        {
            ResponseText.Append(resultText);
            ContentType = contentType;
            StatusCode = statusCode;
            StatusText = statusText;
        }

        public HttpResultContextBase(string statusText, int statusCode, string resultText = "")
        {
            StatusText = statusText;
            StatusCode = statusCode;
            ResponseText.Append(
                string.IsNullOrWhiteSpace(resultText) && statusCode == 404
                    ? string.Format(Html404DefaultBody, statusText, statusCode)
                    : resultText);
        }

        public HttpResultContextBase(string redirectTo, bool permanentRedirect)
        {
            RedirectTo = redirectTo;
            PermanentRedirect = permanentRedirect;
        }

        public static HttpResultContextBase<T> NoContent()
        {
            return new HttpResultContextBase<T>("No Content", 204);
        }

        public static HttpResultContextBase<T> NotFound(string resultText = "Not Found")
        {
            return new HttpResultContextBase<T>(resultText, statusText: "Not Found", statusCode: 404);
        }

        public static HttpResultContextBase<T> Error(string resultText = "Server Error")
        {
            return new HttpResultContextBase<T>(resultText, statusText: "Server Error", statusCode: 500);
        }

        public void ApplyOutputToHttpContext(IHttpContextInfo httpContextInfo)
        {
            if (httpContextInfo.HttpContext != null)
            {
                ApplyOutputToHttpContext(httpContextInfo.HttpContext);

                return;
            }

            if (httpContextInfo.HttpListenerContext != null)
            {
                ApplyOutputToHttpContext(httpContextInfo.HttpListenerContext);

                return;
            }

            throw new InvalidOperationException("There is no HttpContext, or HttpListenerContext defined");
        }

        public void ApplyOutputToHttpContext(HttpContext httpContext)
        {
            if (!string.IsNullOrEmpty(RedirectTo))
            {
                if (!PermanentRedirect)
                {
                    httpContext.Response.Redirect(RedirectTo);
                }
                else
                {
                    httpContext.Response.RedirectPermanent(RedirectTo);
                }

                return;
            }

            if (!IsNotCached && WhenModified > default(DateTime))
            {
                var textIfModifiedSince = httpContext.Request.Headers["If-Modified-Since"];
                if (!string.IsNullOrEmpty(textIfModifiedSince))
                {
                    var modified = DateTime.Parse(textIfModifiedSince);
                    if (modified <= WhenModified)
                    {
                        httpContext.Response.Status = "304 Not Modified";
                        return;
                    }
                }

                // Common headers
                httpContext.Response.Cache.SetCacheability(HttpCacheability.Public);
                httpContext.Response.Cache.SetLastModified(WhenModified);
            }

            if (!string.IsNullOrEmpty(StatusText) && StatusCode > 0)
            {
                httpContext.Response.StatusCode = StatusCode;
                httpContext.Response.Status = StatusText;
            }

            foreach (string name in ResponseHeaders)
            {
                httpContext.Response.AddHeader(name, ResponseHeaders[name]);
            }

            foreach (HttpCookie cookie in ResponseCookies)
            {
                httpContext.Response.Cookies.Add(cookie);
            }

            if (IsNotCached)
            {
                httpContext.Response.ExpiresAbsolute = DateTime.UtcNow.AddDays(-1d);
                httpContext.Response.Expires = -1500;
                httpContext.Response.CacheControl = "no-cache";
                httpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            }
            else
            {
                if (MaxAge.Ticks > 0)
                {
                    httpContext.Response.Cache.SetMaxAge(MaxAge);
                    httpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddTicks(MaxAge.Ticks));
                    httpContext.Response.Cache.SetCacheability(HttpCacheability.Public);
                }
            }

            // This needs to be placed here, or ASP.NET makes an invalid cast on caching.);
            httpContext.Response.Cache.SetOmitVaryStar(true);
            httpContext.Response.Cache.SetVaryByCustom("Accept-Encoding");
            httpContext.Response.AddHeader("Vary", "Accept-Encoding");
            httpContext.Response.Cache.SetNoServerCaching();

            if (IsCompressing)
            {
                string acceptEncoding = httpContext.Request.Headers["Accept-Encoding"];

                if (!string.IsNullOrEmpty(acceptEncoding))
                {
                    acceptEncoding = acceptEncoding.ToLower(CultureInfo.InvariantCulture);

                    if (acceptEncoding.Contains("gzip"))
                    {
                        httpContext.Response.AddHeader("Content-encoding", "gzip");
                        httpContext.Response.Filter = new GZipStream(httpContext.Response.Filter, CompressionMode.Compress);
                    }
                    else if (acceptEncoding.Contains("deflate"))
                    {
                        httpContext.Response.AddHeader("Content-encoding", "deflate");
                        httpContext.Response.Filter = new DeflateStream(httpContext.Response.Filter, CompressionMode.Compress);
                    }
                }
            }

            if (!string.IsNullOrEmpty(FileName))
            {
                httpContext.Response.ContentType = ContentType;
                httpContext.Response.WriteFile(FileName);
                return;
            }

            if (ResponseText.Length > 0)
            {
                httpContext.Response.ContentType = ContentType + "; charset=utf-8";
                httpContext.Response.ContentEncoding = Encoding.UTF8;
                httpContext.Response.Write(ResponseText.ToString());
            }
        }

        public void ApplyOutputToHttpContext(HttpListenerContext httpContext)
        {
            if (!string.IsNullOrEmpty(RedirectTo))
            {
                if (!PermanentRedirect)
                {
                    httpContext.Response.Redirect(RedirectTo);
                }
                else
                {
                    throw new NotSupportedException("Permanent redirect is not yet supported");
                    // httpContext.Response.RedirectPermanent(this.RedirectTo);
                }

                return;
            }

            if (!string.IsNullOrEmpty(StatusText) && StatusCode > 0)
            {
                httpContext.Response.StatusCode = StatusCode;
                httpContext.Response.StatusDescription = StatusText;
            }

            if (!IsNotCached && WhenModified > default(DateTime))
            {
                var textIfModifiedSince = httpContext.Request.Headers["If-Modified-Since"];
                if (!string.IsNullOrEmpty(textIfModifiedSince))
                {
                    var modified = DateTime.Parse(textIfModifiedSince);
                    if (modified <= WhenModified)
                    {
                        httpContext.Response.StatusDescription = "Not Modified";
                        httpContext.Response.StatusCode = 304;
                        return;
                    }
                }

                // Common headers
                httpContext.Response.Headers.Add(HttpRequestHeader.CacheControl, "public");
                httpContext.Response.Headers.Add(HttpRequestHeader.LastModified, WhenModified.ToUniversalTime().ToString("R"));
            }

            foreach (string name in ResponseHeaders)
            {
                httpContext.Response.AddHeader(name, ResponseHeaders[name]);
            }

            foreach (HttpCookie cookie in ResponseCookies)
            {
                httpContext.Response.Cookies.Add(new Cookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain)
                                                 {
                                                     Expires = cookie.Expires,
                                                     HttpOnly = cookie.HttpOnly,
                                                     Secure = cookie.Secure
                                                 });
            }

            if (IsNotCached)
            {
                httpContext.Response.Headers.Add(HttpResponseHeader.Expires, DateTime.UtcNow.AddDays(-1d).ToString("R"));
                httpContext.Response.Headers.Add(HttpResponseHeader.CacheControl, "no-cache");
            }
            else
            {
                if (MaxAge.Ticks > 0)
                {
                    httpContext.Response.Headers.Add(HttpResponseHeader.Expires, DateTime.UtcNow.AddTicks(MaxAge.Ticks).ToString("R"));
                    httpContext.Response.Headers.Add(HttpResponseHeader.CacheControl, "max-age=" + MaxAge.TotalSeconds);
                }
            }

            var buffer = new byte[0];
            if (!string.IsNullOrEmpty(FileName))
            {
                httpContext.Response.ContentType = ContentType;
                buffer = File.ReadAllBytes(FileName);
            }

            if (ResponseText.Length > 0)
            {
                buffer = Encoding.UTF8.GetBytes(ResponseText.ToString());
            }

            if (IsCompressing)
            {
                var acceptEncoding = httpContext.Request.Headers["Accept-Encoding"];

                if (!string.IsNullOrEmpty(acceptEncoding))
                {
                    acceptEncoding = acceptEncoding.ToLower(CultureInfo.InvariantCulture);

                    if (acceptEncoding.Contains("gzip"))
                    {
                        using (var ms = new MemoryStream())
                        {
                            using (var zip = new GZipStream(ms, CompressionMode.Compress, true))
                            {
                                zip.Write(buffer, 0, buffer.Length);
                            }

                            buffer = ms.ToArray();
                        }

                        httpContext.Response.AddHeader("Content-encoding", "gzip");
                    }
                    else if (acceptEncoding.Contains("deflate"))
                    {
                        using (var ms = new MemoryStream())
                        {
                            using (var zip = new DeflateStream(ms, CompressionMode.Compress, true))
                            {
                                zip.Write(buffer, 0, buffer.Length);
                            }

                            buffer = ms.ToArray();
                        }

                        httpContext.Response.AddHeader("Content-encoding", "deflate");
                    }

                    httpContext.Response.AddHeader("Vary", "Accept-Encoding");
                }
            }

            httpContext.Response.ContentType = ContentType + "; charset=utf-8";
            httpContext.Response.ContentEncoding = Encoding.UTF8;
            httpContext.Response.ContentLength64 = buffer.LongLength;
            httpContext.Response.OutputStream.Write(buffer, 0, buffer.Length);
        }

        public T AccessControlAllowOriginAll()
        {
            ResponseHeaders.Add("Access-Control-Allow-Origin", "*");
            return (T)this;
        }

        public T NoCache()
        {
            IsNotCached = true;
            return (T)this;
        }

        public T Cache(TimeSpan maxAge)
        {
            MaxAge = maxAge;
            return (T)this;
        }

        public T CompressRequest()
        {
            IsCompressing = true;
            return (T)this;
        }

        public T RenderOnlyIfIsModifiedSince(DateTime whenModified)
        {
            WhenModified = whenModified;
            return (T)this;
        }

        public T AddHeader(string key, string value)
        {
            ResponseHeaders.Add(key, value);
            return (T)this;
        }

        public T SendFile(string fileName)
        {
            FileName = fileName;
            return (T)this;
        }

        public void AggregateToThis(T context)
        {
            ResponseText.Append(context.ResponseText);

            foreach (string name in context.ResponseHeaders)
            {
                AddHeader(name, context.ResponseHeaders[name]);
            }

            foreach (HttpCookie cookie in context.ResponseCookies)
            {
                ResponseCookies.Add(cookie);
            }
        }
    }
}
