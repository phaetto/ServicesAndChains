namespace Services.Communication.Protocol
{
    using Chains.Play;

    public class SendNoReply : Send<ClientConnectionContext>
    {
        public SendNoReply(
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

        public SendNoReply(
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

        public SendNoReply(
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

        protected override ClientConnectionContext SendData(ClientConnectionContext context, ExecutableActionSpecification[] specifications)
        {
            context.ClientProtocolStack.SendStream(specifications.SerializeToJson());
            return context;
        }
    }
}
