namespace Services.Communication.Tcp
{
    using System;
    using System.IO;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using Chains;
    using Chains.Play;

    [Obsolete]
    public class Send<T> : IChainableAction<ClientConnectionContext, T>
    {
        private readonly ExecutableActionSpecification[] specifications;

        private readonly int timesToRetry;

        private readonly int intervalBetweenTimesInMilliseconds;

        private readonly int randomAmountForIntervalBetweenTimesInMilliseconds;

        private readonly Random random = new Random();

        public Send(
            IReproducible action,
            int timesToRetry = 3,
            int intervalBetweenTimesInMilliseconds = 200,
            int randomAmountForIntervalBetweenTimesInMilliseconds = 50)
            : this(new[]
                   {
                       action.GetInstanceSpec()
                   },
                timesToRetry,
                intervalBetweenTimesInMilliseconds,
                randomAmountForIntervalBetweenTimesInMilliseconds)
        {
        }

        public Send(
            ExecutableActionSpecification specification,
            int timesToRetry = 3,
            int intervalBetweenTimesInMilliseconds = 200,
            int randomAmountForIntervalBetweenTimesInMilliseconds = 50)
            : this(new[]
                   {
                       specification
                   },
                timesToRetry,
                intervalBetweenTimesInMilliseconds,
                randomAmountForIntervalBetweenTimesInMilliseconds)
        {
        }

        public Send(
            ExecutableActionSpecification[] specifications,
            int timesToRetry = 3,
            int intervalBetweenTimesInMilliseconds = 200,
            int randomAmountForIntervalBetweenTimesInMilliseconds = 50)
        {
            this.specifications = specifications;
            this.timesToRetry = timesToRetry;
            this.intervalBetweenTimesInMilliseconds = intervalBetweenTimesInMilliseconds;
            this.randomAmountForIntervalBetweenTimesInMilliseconds = randomAmountForIntervalBetweenTimesInMilliseconds;
        }

        public T Act(ClientConnectionContext context)
        {
            var retries = timesToRetry;

            while (true)
            {
                try
                {
                    lock (context)
                    {
                        if (!(context.ClientSocket.Poll(0, SelectMode.SelectWrite)
                                && context.ClientSocket.Available == 0)) // !context.ClientSocket.Connected)
                        {
                            throw new SocketException((int)SocketError.ConnectionAborted);
                        }

                        using (var socketStream = new NetworkStream(context.ClientSocket))
                        {
                            socketStream.ReadTimeout = 60 * 1000;
                            socketStream.WriteTimeout = 60 * 1000;

                            using (var writer = new StreamWriter(socketStream, Encoding.ASCII))
                            {
                                SendToServer(writer);
                                writer.Flush();

                                using (var reader = new StreamReader(socketStream, Encoding.ASCII))
                                {
                                    var line = reader.ReadLine();
                                    return GetResponseFromServer(context, line);
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    if (retries == 0)
                    {
                        context.Close();
                        throw;
                    }

                    Thread.Sleep(
                        intervalBetweenTimesInMilliseconds
                            + random.Next(
                                -randomAmountForIntervalBetweenTimesInMilliseconds,
                                randomAmountForIntervalBetweenTimesInMilliseconds));

                    if (retries > 0)
                    {
                        --retries;
                        context.Do(new RetryConnection());
                    }
                }
            }
        }

        private void SendToServer(StreamWriter writer)
        {
            writer.Write(string.Format("TcpCommand {0}\r\n", specifications.SerializeToJson()));
            writer.Flush();
        }

        private static T GetResponseFromServer(ClientConnectionContext context, string line)
        {
            var responseObject = DeserializableSpecification<ExecutableActionSpecification>.DeserializeFromJson(line);

            var exception = responseObject.Data as Exception;
            if (exception != null)
            {
                throw exception;
            }

            if (typeof(T) == typeof(ClientConnectionContext))
            {
                return (T)Convert.ChangeType(context, typeof(T));
            }

            return (T)Convert.ChangeType(responseObject.Data, typeof(T));
        }
    }

    public class Send : Send<ClientConnectionContext>
    {
        public Send(
            IReproducible action,
            int timesToRetry = 3,
            int intervalBetweenTimesInMilliseconds = 200,
            int randomAmountForIntervalBetweenTimesInMilliseconds = 50)
            : base(
                action,
                timesToRetry,
                intervalBetweenTimesInMilliseconds,
                randomAmountForIntervalBetweenTimesInMilliseconds)
        {
        }

        public Send(
            ExecutableActionSpecification specification,
            int timesToRetry = 3,
            int intervalBetweenTimesInMilliseconds = 200,
            int randomAmountForIntervalBetweenTimesInMilliseconds = 50)
            : base(
                specification,
                timesToRetry,
                intervalBetweenTimesInMilliseconds,
                randomAmountForIntervalBetweenTimesInMilliseconds)
        {
        }

        public Send(
            ExecutableActionSpecification[] specifications,
            int timesToRetry = 3,
            int intervalBetweenTimesInMilliseconds = 200,
            int randomAmountForIntervalBetweenTimesInMilliseconds = 50)
            : base(
                specifications,
                timesToRetry,
                intervalBetweenTimesInMilliseconds,
                randomAmountForIntervalBetweenTimesInMilliseconds)
        {
        }
    }
}
