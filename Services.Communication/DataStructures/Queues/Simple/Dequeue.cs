namespace Services.Communication.DataStructures.Queues.Simple
{
    using Chains.Play;

    public sealed class Dequeue : RemotableAction<ExecutableActionSpecification, QueueContext>
    {
        public override ExecutableActionSpecification Act(QueueContext context)
        {
            var actionSpecification = new ExecutableActionSpecification();
            context.Queue.TryDequeue(out actionSpecification);
            return actionSpecification;
        }
    }
}
