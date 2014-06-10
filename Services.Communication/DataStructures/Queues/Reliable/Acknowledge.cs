namespace Services.Communication.DataStructures.Queues.Reliable
{
    using Chains;
    using Chains.Play;

    public sealed class Acknowledge : ReproducibleWithSerializableData<int>, IChainableAction<QueueContext, QueueContext>
    {
        public Acknowledge(int id)
            : base(id)
        {
        }

        public QueueContext Act(QueueContext context)
        {
            lock (context.UnacknowledgedQueuedItemsLock)
            {
                context.UnacknowledgedQueuedItems.RemoveAll(x => x.Id == Data);
            }

            return context;
        }
    }
}
