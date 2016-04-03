namespace Chains.Play.Web.HttpListener.Handlers
{
    using System;
    using System.IO;
    using System.Net;
    using Chains.Play.Web;

    public sealed class DefaultFileHttpHandler : HttpRestHandler
    {
        private readonly string defaultFile;

        private readonly string directory;

        public DefaultFileHttpHandler(HttpServer httpServer, string defaultFile = "index.html", string path = "/", string directory = null)
            : base(httpServer, path, defaultFile)
        {
            this.defaultFile = defaultFile;
            this.directory = directory;
        }

        public override void Get(HttpListenerContext context)
        {
            var path = $"{directory}{Path.DirectorySeparatorChar}{defaultFile}";

            new HttpResultContext().SendFile(path)
                .AccessControlAllowOriginAll()
                .ApplyOutputToHttpContext(context);
        }

        public override void Post(HttpListenerContext context)
        {
            throw new NotImplementedException();
        }

        public override void Put(HttpListenerContext context)
        {
            throw new NotImplementedException();
        }

        public override void Delete(HttpListenerContext context)
        {
            throw new NotImplementedException();
        }
    }
}
