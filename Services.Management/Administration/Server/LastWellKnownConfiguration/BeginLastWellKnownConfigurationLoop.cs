namespace Services.Management.Administration.Server.LastWellKnownConfiguration
{
    using System.Threading.Tasks;
    using Chains;

    public sealed class BeginLastWellKnownConfigurationLoop : IChainableAction<LastWellKnownConfigurationContext, Task<LastWellKnownConfigurationContext>>
    {
        public Task<LastWellKnownConfigurationContext> Act(LastWellKnownConfigurationContext context)
        {
            string serviceToProcess;
            if (context.ServicesToCreateSnapshotConcurrentQueue.TryDequeue(out serviceToProcess))
            {
                context.Do(new CreateLastWellKnownConfigurationSnapshot(serviceToProcess));

                TaskEx.FromResult(context);
            }


            return TaskEx.Delay(1000).ContinueWith(x => context.Do(this)).Unwrap();
        }
    }
}
