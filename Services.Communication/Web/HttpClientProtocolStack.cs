namespace Services.Communication.Web
{
    using System;
    using System.Collections.Specialized;
    using System.Net;
    using Chains.Play.Web;
    using Services.Communication.Protocol;

    public class HttpClientProtocolStack : IClientProtocolStack
    {
        public const string HeaderKeyAndValue = "no-content";

        private Client context;

        public void OpenClientConnection(Client context)
        {
            if (context.Path == null)
            {
                throw new InvalidOperationException("The http protocol stack needs to include the 'path' in client (non-null).");
            }

            this.context = context;
        }

        public void CloseClientConnection()
        {
            this.context = null;
        }

        public void RetryClientConnection(Client context)
        {
            OpenClientConnection(context);
        }

        public void SendStream(string data)
        {
            var server = $"http://{context.Hostname}";
            if (context.Port != 80)
            {
                server += $":{context.Port}";
            }

            var headers = new NameValueCollection();
            headers.Add(HeaderKeyAndValue, HeaderKeyAndValue);

            var response = HttpRequest.DoRequest(
                $"{server}{context.Path}",
                "post",
                data,
                headers: headers);

            if (response.HasError)
            {
                throw new WebException(response.Response);
            }
        }

        public string SendAndReceiveStream(string data)
        {
            var server = $"http://{context.Hostname}";
            if (context.Port != 80)
            {
                server += $":{context.Port}";
            }

            var response = HttpRequest.DoRequest($"{server}{context.Path}", "post", data);

            if (response.HasError)
            {
                throw new WebException(response.Response);
            }

            return response.Response;
        }
    }
}
