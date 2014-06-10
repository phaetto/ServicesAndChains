namespace Services.Communication.DataStructures.Queues.Simple
{
    using Chains.Play;

    public sealed class Dequeue : RemotableAction<ExecutableActionSpecification, QueueContext>
    {
        protected override ExecutableActionSpecification ActRemotely(QueueContext context)
        {
            var actionSpecification = new ExecutableActionSpecification();
            context.Queue.TryDequeue(out actionSpecification);
            return actionSpecification;
        }
    }
}
