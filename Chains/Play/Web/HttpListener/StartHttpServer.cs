namespace Chains.Play.Web.HttpListener
{
    using System;
    using System.Collections.Generic;
    using System.Net;

    public sealed class StartHttpServer : IChainableAction<ServerHost, HttpServer>
    {
        private readonly IEnumerable<string> paths;

        private readonly int maxThreads;

        public StartHttpServer()
            : this(new string[0], 1)
        {
        }

        public StartHttpServer(IEnumerable<string> paths) : this(paths, 1)
        {
        }

        public StartHttpServer(IEnumerable<string> paths, int maxThreads)
        {
            this.paths = paths;
            this.maxThreads = maxThreads;
        }

        public HttpServer Act(ServerHost context)
        {
            if (!HttpListener.IsSupported)
            {
                throw new NotSupportedException("HttpListener is not supported in your platform.");
            }

            return new HttpServer(context, paths, maxThreads);
        }
    }
}
