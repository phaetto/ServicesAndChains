namespace Services.Communication.DataStructures.Queues.Reliable
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using Chains;
    using Chains.Play;

    public sealed class QueueContext : Chain<QueueContext>, IDisposable
    {
        public readonly ConcurrentQueue<ExecutableActionSpecification> Queue = new ConcurrentQueue<ExecutableActionSpecification>();

        public readonly List<UnacknowledgedItem> UnacknowledgedQueuedItems = new List<UnacknowledgedItem>();

        public readonly object UnacknowledgedQueuedItemsLock = new object();

        internal Random random = new Random();

        private Thread AcknowledgeThread;

        public QueueContext()
        {
            AcknowledgeThread = new Thread(AcknowledgeThreadExecution);
            AcknowledgeThread.Start();
        }

        private void AcknowledgeThreadExecution()
        {
            while (true)
            {
                foreach (var unacknowledgedItem in UnacknowledgedQueuedItems)
                {
                    var timeExists = DateTime.UtcNow - unacknowledgedItem.DateIssued;
                    if (timeExists.TotalMinutes > 5)
                    {
                        lock (UnacknowledgedQueuedItemsLock)
                        {
                            UnacknowledgedQueuedItems.RemoveAll(x => x.Id == unacknowledgedItem.Id);
                            Queue.Enqueue(unacknowledgedItem.Specification);
                        }
                    }
                }

                Thread.Sleep(5 * 60 * 1000);
            }
        }

        public void Dispose()
        {
            AcknowledgeThread.Abort();
        }
    }
}
