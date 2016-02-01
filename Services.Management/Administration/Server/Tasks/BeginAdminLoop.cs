namespace Services.Management.Administration.Server.Tasks
{
    using Chains;
    using Chains.Play.Streams.Timer;

    public sealed class BeginAdminLoop : IChainableAction<AdministrationContext, AdministrationContext>
    {
        public const int IntervalInMilliseconds = 1000;

        private readonly bool hasJustStarted;

        public BeginAdminLoop(bool hasJustStarted)
        {
            this.hasJustStarted = hasJustStarted;
        }

        public AdministrationContext Act(AdministrationContext context)
        {
            if (hasJustStarted)
            {
                context.Do(new CleanUpAdminReports()).Do(new StartServicesExtensionsWellKnownService());
                context.TimerStreamScheduler.ScheduleActionCall(new BeginAdminLoop(false), IntervalInMilliseconds,
                    TimerScheduledCallType.Once);

                return context;
            }

            context.Do(new CleanUpAdminReports());

            context.TimerStreamScheduler.ScheduleActionCall(this, IntervalInMilliseconds, TimerScheduledCallType.Once);

            return context;
        }
    }
}
