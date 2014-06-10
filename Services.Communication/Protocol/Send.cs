namespace Services.Communication.Protocol
{
    using System;
    using System.Threading;
    using Chains;
    using Chains.Play;

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
                        var data = context.ClientProtocolStack.SendAndReceiveStream(
                                specifications.SerializeToJson());

                        return GetResponseFromServer(context, data);
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

        private static T GetResponseFromServer(ClientConnectionContext context, string data)
        {
            var responseObject = DeserializableSpecification<ExecutableActionSpecification>.DeserializeFromJson(data);

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
