namespace Services.Communication.DataStructures.Queues.Reliable
{
    using Chains;
    using Chains.Play;

    public sealed class Enqueue : ReproducibleWithData<ExecutableActionSpecification>, IChainableAction<QueueContext, QueueContext>
    {
        public Enqueue(ExecutableActionSpecification data)
            : base(data)
        {
        }

        public QueueContext Act(QueueContext context)
        {
            context.Queue.Enqueue(Data);
            return context;
        }
    }
}
