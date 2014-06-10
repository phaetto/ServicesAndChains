namespace Services.Communication.Protocol
{
    using System;
    using System.Threading;
    using Chains;

    public class SendRaw : IChainableAction<ClientConnectionContext, string>
    {
        private readonly string jsonListData;

        private readonly int timesToRetry;

        private readonly int intervalBetweenTimesInMilliseconds;

        private readonly int randomAmountForIntervalBetweenTimesInMilliseconds;

        private readonly Random random = new Random();

        public SendRaw(
            string jsonListData,
            int timesToRetry = 3,
            int intervalBetweenTimesInMilliseconds = 200,
            int randomAmountForIntervalBetweenTimesInMilliseconds = 50)
        {
            this.jsonListData = jsonListData;
            this.timesToRetry = timesToRetry;
            this.intervalBetweenTimesInMilliseconds = intervalBetweenTimesInMilliseconds;
            this.randomAmountForIntervalBetweenTimesInMilliseconds = randomAmountForIntervalBetweenTimesInMilliseconds;
        }

        public string Act(ClientConnectionContext context)
        {
            var retries = timesToRetry;

            while (true)
            {
                try
                {
                    lock (context)
                    {
                        return context.ClientProtocolStack.SendAndReceiveStream(jsonListData);
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
    }
}
