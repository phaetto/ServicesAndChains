namespace Services.Communication.Web
{
    using System;
    using System.Net;
    using System.Threading;
    using Chains;
    using Chains.Play;
    using Chains.Play.Web;

    public class Send<T> : IChainableAction<Client, T>
    {
        private readonly ExecutableActionSpecification[] specifications;

        private readonly int timesToRetry;

        private readonly int intervalBetweenTimesInMilliseconds;

        private readonly int randomAmountForIntervalBetweenTimesInMilliseconds;

        private readonly Random random = new Random();

        private readonly string path;

        public Send(
            string path,
            IReproducible action,
            int timesToRetry = 3,
            int intervalBetweenTimesInMilliseconds = 200,
            int randomAmountForIntervalBetweenTimesInMilliseconds = 50)
            : this(path,
                new[]
                {
                    action.GetInstanceSpec()
                },
                timesToRetry,
                intervalBetweenTimesInMilliseconds,
                randomAmountForIntervalBetweenTimesInMilliseconds)
        {
        }

        public Send(
            string path,
            ExecutableActionSpecification specification,
            int timesToRetry = 3,
            int intervalBetweenTimesInMilliseconds = 200,
            int randomAmountForIntervalBetweenTimesInMilliseconds = 50)
            : this(path,
                new[]
                {
                    specification
                },
                timesToRetry,
                intervalBetweenTimesInMilliseconds,
                randomAmountForIntervalBetweenTimesInMilliseconds)
        {
        }

        public Send(
            string path,
            ExecutableActionSpecification[] specifications,
            int timesToRetry = 3,
            int intervalBetweenTimesInMilliseconds = 200,
            int randomAmountForIntervalBetweenTimesInMilliseconds = 50)
        {
            this.specifications = specifications;
            this.timesToRetry = timesToRetry;
            this.intervalBetweenTimesInMilliseconds = intervalBetweenTimesInMilliseconds;
            this.randomAmountForIntervalBetweenTimesInMilliseconds = randomAmountForIntervalBetweenTimesInMilliseconds;
            this.path = path;
        }

        public T Act(Client context)
        {
            var retries = timesToRetry;

            while (true)
            {
                try
                {
                    var server = "http://" + context.Hostname;
                    if (context.Port != 80)
                    {
                        server += ":" + context.Port;
                    }

                    var response = HttpRequest.DoRequest(server + path, "post", specifications.SerializeToJson());

                    if (response.HasError)
                    {
                        throw new WebException(response.Response);
                    }

                    var responseExecutableActionSpecification =
                        DeserializableSpecification<ExecutableActionSpecification>.DeserializeFromJson(
                            response.Response);

                    if (typeof(T) == typeof(Client)
                        || responseExecutableActionSpecification.DataType == typeof(bool).FullName)
                    {
                        return (T)Convert.ChangeType(context, typeof(T));
                    }

                    return (T)Convert.ChangeType(responseExecutableActionSpecification.Data, typeof(T));
                }
                catch (Exception)
                {
                    if (retries == 0)
                    {
                        throw;
                    }

                    Thread.Sleep(
                        intervalBetweenTimesInMilliseconds
                            + random.Next(
                                -randomAmountForIntervalBetweenTimesInMilliseconds,
                                randomAmountForIntervalBetweenTimesInMilliseconds));

                    --retries;
                }
            }
        }
    }

    public class Send : Send<Client>
    {
        public Send(
            string path,
            IReproducible action,
            int timesToRetry = 3,
            int intervalBetweenTimesInMilliseconds = 200,
            int randomAmountForIntervalBetweenTimesInMilliseconds = 50)
            : base(
                path,
                action,
                timesToRetry,
                intervalBetweenTimesInMilliseconds,
                randomAmountForIntervalBetweenTimesInMilliseconds)
        {
        }

        public Send(
            string path,
            ExecutableActionSpecification specification,
            int timesToRetry = 3,
            int intervalBetweenTimesInMilliseconds = 200,
            int randomAmountForIntervalBetweenTimesInMilliseconds = 50)
            : base(
                path,
                specification,
                timesToRetry,
                intervalBetweenTimesInMilliseconds,
                randomAmountForIntervalBetweenTimesInMilliseconds)
        {
        }

        public Send(
            string path,
            ExecutableActionSpecification[] specifications,
            int timesToRetry = 3,
            int intervalBetweenTimesInMilliseconds = 200,
            int randomAmountForIntervalBetweenTimesInMilliseconds = 50)
            : base(
                path,
                specifications,
                timesToRetry,
                intervalBetweenTimesInMilliseconds,
                randomAmountForIntervalBetweenTimesInMilliseconds)
        {
        }
    }
}
