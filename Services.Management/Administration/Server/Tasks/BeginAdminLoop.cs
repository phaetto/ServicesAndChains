namespace Services.Management.Administration.Server.Tasks
{
    using System.Threading.Tasks;
    using Chains;

    public sealed class BeginAdminLoop : IChainableAction<AdministrationContext, Task<AdministrationContext>>
    {
        private readonly bool hasJustStarted;

        public BeginAdminLoop(bool hasJustStarted)
        {
            this.hasJustStarted = hasJustStarted;
        }

        public Task<AdministrationContext> Act(AdministrationContext context)
        {
            if (hasJustStarted)
            {
                context.Do(new CleanUpAdminReports()).Do(new StartServicesExtensionsWellKnownService());
                return TaskEx.DelayWithCarbageCollection(1000).ContinueWith(x => context.Do(new BeginAdminLoop(false))).Unwrap();
            }

            context.Do(new CleanUpAdminReports());

            return TaskEx.DelayWithCarbageCollection(1000).ContinueWith(x => context.Do(this)).Unwrap();
        }
    }
}
