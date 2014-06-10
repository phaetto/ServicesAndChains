namespace Services.Communication.DataStructures.Queues.Reliable
{
    using System;
    using Chains.Play;

    public sealed class Dequeue : RemotableAction<UnacknowledgedItem, QueueContext>
    {
        protected override UnacknowledgedItem ActRemotely(QueueContext context)
        {
            var actionSpecification = new ExecutableActionSpecification();
            lock (context.UnacknowledgedQueuedItemsLock)
            {
                if (context.Queue.TryDequeue(out actionSpecification))
                {
                    var unacknowledgeItem = new UnacknowledgedItem
                                            {
                                                Specification = actionSpecification,
                                                DateIssued = DateTime.UtcNow,
                                                Id = context.random.Next(int.MaxValue)
                                            };
                    context.UnacknowledgedQueuedItems.Add(unacknowledgeItem);

                    return unacknowledgeItem;
                }

                return null;
            }
        }
    }
}
