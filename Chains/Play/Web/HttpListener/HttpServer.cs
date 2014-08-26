namespace Chains.Play.Web.HttpListener
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading;
    using Chains.Play.Modules;

    public sealed class HttpServer : ChainWithParent<HttpServer, ServerHost>, IDisposable, IModular
    {
        private readonly HttpListener listener;

        private readonly Thread listenerThread;

        private readonly Thread[] workers;

        private readonly ManualResetEvent stop;

        private readonly ManualResetEvent ready;

        private readonly Queue<HttpListenerContext> queue;

        public List<AbstractChain> Modules { get; set; }

        public HttpServer(ServerHost chain, IEnumerable<string> paths = null, int maxThreads = 1)
            : base(chain)
        {
            workers = new Thread[maxThreads];
            queue = new Queue<HttpListenerContext>();
            stop = new ManualResetEvent(false);
            ready = new ManualResetEvent(false);
            listener = new HttpListener();
            listenerThread = new Thread(HandleRequests);

            Modules = new List<AbstractChain>();

            var serverBaseUri = "http://" + this.Parent.Parent.Hostname
                + (this.Parent.Parent.Port != 80 ? ":" + this.Parent.Parent.Port : string.Empty);

            if (paths != null)
            {
                foreach (var path in paths)
                {
                    listener.Prefixes.Add(serverBaseUri + path);
                }
            }

            listener.Start();
            listenerThread.Start();

            for (var i = 0; i < workers.Length; i++)
            {
                workers[i] = new Thread(Worker);
                workers[i].Start();
            }
        }

        public void Dispose()
        {
            Stop();
        }

        public void Stop()
        {
            stop.Set();
            listenerThread.Join();

            foreach (var worker in workers)
            {
                worker.Join();
            }

            listener.Stop();
        }

        public void AddPath(string path)
        {
            var serverBaseUri = "http://" + this.Parent.Parent.Hostname
                + (this.Parent.Parent.Port != 80 ? ":" + this.Parent.Parent.Port : string.Empty);

            listener.Prefixes.Add(serverBaseUri + path);
        }

        public void AddPath(Uri uri)
        {
            listener.Prefixes.Add(uri.ToString());
        }

        public void RemovePath(string path)
        {
            var serverBaseUri = "http://" + this.Parent.Parent.Hostname
                + (this.Parent.Parent.Port != 80 ? ":" + this.Parent.Parent.Port : string.Empty);

            listener.Prefixes.Remove(serverBaseUri + path);
        }

        public void RemovePath(Uri uri)
        {
            listener.Prefixes.Remove(uri.ToString());
        }

        private void HandleRequests()
        {
            while (listener.IsListening)
            {
                var context = listener.BeginGetContext(ContextReady, null);

                if (0 == WaitHandle.WaitAny(
                    new[]
                    {
                        stop, context.AsyncWaitHandle
                    }))
                {
                    return;
                }
            }
        }

        private void ContextReady(IAsyncResult ar)
        {
            try
            {
                lock (queue)
                {
                    queue.Enqueue(listener.EndGetContext(ar));
                    ready.Set();
                }
            }
            catch
            {
            }
        }

        private void Worker()
        {
            var wait = new[]
                       {
                           ready, stop
                       };

            while (0 == WaitHandle.WaitAny(wait))
            {
                HttpListenerContext httpListenerContext;
                lock (queue)
                {
                    if (queue.Count > 0)
                    {
                        httpListenerContext = queue.Dequeue();
                    }
                    else
                    {
                        ready.Reset();
                        continue;
                    }
                }

                var responseHasBeenHandled = false;
                foreach (IHttpRequestHandler module in this.Modules)
                {
                    try
                    {
                        if (module.ResolveRequest(httpListenerContext))
                        {
                            responseHasBeenHandled = true;
                            httpListenerContext.Response.Close();
                            break;
                        }
                    }
                    catch (HttpListenerException)
                    {
                        responseHasBeenHandled = true;

                        try
                        {
                            httpListenerContext.Response.Close();
                        }
                        catch (InvalidOperationException)
                        {
                            // Connection closed
                        }

                        break;
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            responseHasBeenHandled = true;
                            if (httpListenerContext.Request.Url.Host.ToLowerInvariant() == "localhost")
                            {
                                HttpResultContext.Error(
                                    string.Format(
                                        "<h1>{0}</h1><p>{1}</p><p>{2}</p><p>{3}</p>",
                                        ex.Message,
                                        ex.GetType().FullName,
                                        ex.Source,
                                        ex.StackTrace)).ApplyOutputToHttpContext(httpListenerContext);
                            }
                            else
                            {
                                HttpResultContext.Error().ApplyOutputToHttpContext(httpListenerContext);
                            }

                            httpListenerContext.Response.Close();
                        }
                        catch (InvalidOperationException)
                        {
                            // Connection closed
                        }

                        break;
                    }
                }

                if (!responseHasBeenHandled)
                {
                    try
                    {
                        HttpResultContext.NotFound().ApplyOutputToHttpContext(httpListenerContext);
                        httpListenerContext.Response.Close();
                    }
                    catch (InvalidOperationException)
                    {
                        // Connection closed
                    }
                }
            }
        }
    }
}
