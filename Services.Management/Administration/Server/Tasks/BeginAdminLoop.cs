namespace Services.Management.Administration.Server.Tasks
{
    using Chains;
    using Chains.Play.Streams.Timer;

    public sealed class BeginAdminLoop : IChainableAction<AdministrationContext, AdministrationContext>
    {
        public const int IntervalInMilliseconds = 1000;
        public const int NinetySecondsIntervalInMilliseconds = 90 * 1000;

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

                context.TimerStreamScheduler.ScheduleActionCall(new RemoveWellKnownStartedServicesLoop(), NinetySecondsIntervalInMilliseconds,
                    TimerScheduledCallType.Once);

                return context;
            }

            context.TimerStreamScheduler.ScheduleActionCall(new CleanUpAdminReports(), IntervalInMilliseconds, TimerScheduledCallType.Recurrent);

            context.Do(new CleanUpAdminReports());

            return context;
        }
    }
}
