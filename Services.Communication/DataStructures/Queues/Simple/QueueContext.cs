namespace Services.Communication.DataStructures.Queues.Simple
{
    using System.Collections.Concurrent;
    using Chains;
    using Chains.Play;

    public sealed class QueueContext : Chain<QueueContext>
    {
        public ConcurrentQueue<ExecutableActionSpecification> Queue = new ConcurrentQueue<ExecutableActionSpecification>();
    }
}
