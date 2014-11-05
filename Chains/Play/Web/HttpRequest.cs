namespace Chains.Play.Web
{
    using System;
    using System.Collections.Specialized;
    using System.IO;
    using System.Net;
    using System.Text;

    public static class HttpRequest
    {
        public const string FormUrlEncodedContentType = "application/x-www-form-urlencoded";

        public static HttpResponseResult DoRequest(
            string url,
            string method = "get",
            string postData = null,
            string contentType = null,
            NameValueCollection headers = null)
        {
            var responseData = new HttpResponseResult();
            try
            {
                var webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.Method = method.ToUpperInvariant();

                if (webRequest.Method != "GET")
                {
                    if (string.IsNullOrWhiteSpace(postData))
                    {
                        throw new InvalidOperationException("Post data cannot be null when doing a non-get request");
                    }

                    var postBytes = Encoding.UTF8.GetBytes(postData);

                    if (!string.IsNullOrWhiteSpace(contentType))
                    {
                        webRequest.ContentType = contentType.ToLowerInvariant();
                    }

                    if (headers != null)
                    {
                        webRequest.Headers.Add(headers);
                    }

                    webRequest.ContentLength = postBytes.Length;

                    using (var postStream = webRequest.GetRequestStream())
                    {
                        postStream.Write(postBytes, 0, postBytes.Length);
                        postStream.Close();
                    }
                }

                using (var webResponse = (HttpWebResponse)webRequest.GetResponse())
                {
                    using (var responseStream = webResponse.GetResponseStream())
                    {
                        responseData.Response = new StreamReader(responseStream).ReadToEnd();

                        return responseData;
                    }
                }
            }
            catch (WebException exception)
            {
                var text = GetWebExceptionResponse(exception);
                if (!string.IsNullOrWhiteSpace(text))
                {
                    responseData.Response = text;
                    responseData.HasError = true;
                    return responseData;
                }

                throw;
            }
        }

        private static string GetWebExceptionResponse(WebException exception)
        {
            using (var response = exception.Response)
            {
                if (response == null)
                {
                    return null;
                }

                using (var data = response.GetResponseStream())
                {
                    if (data == null)
                    {
                        return null;
                    }

                    var text = new StreamReader(data).ReadToEnd();

                    return text;
                }
            }
        }
    }
}
