namespace Services.Management.Administration.Server.LastWellKnownConfiguration
{
    using Chains;

    public sealed class EnqueueService : IChainableAction<LastWellKnownConfigurationContext, LastWellKnownConfigurationContext>
    {
        private readonly string serviceToCheck;

        public EnqueueService(string serviceToCheck)
        {
            this.serviceToCheck = serviceToCheck;
        }

        public LastWellKnownConfigurationContext Act(LastWellKnownConfigurationContext context)
        {
            context.ServicesToCreateSnapshotConcurrentQueue.Enqueue(serviceToCheck);

            return context;
        }
    }
}
