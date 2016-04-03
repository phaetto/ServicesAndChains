namespace Services.Management.Administration.Server.Tasks
{
    using Chains;
    using Chains.Play.Streams.Timer;
    using Services.Management.Administration.Worker;

    public sealed class RemoveWellKnownStartedServicesLoop : IChainableAction<AdministrationContext, AdministrationContext>
    {
        public const int IntervalInMilliseconds = 5000;

        public AdministrationContext Act(AdministrationContext context)
        {
            var reports = context.ReportData;

            if (reports.ContainsKey(StartServicesExtensionsWellKnownService.Id))
            {
                if (reports[StartServicesExtensionsWellKnownService.Id].WorkerState != WorkUnitState.Stopping)
                {
                    context.TimerStreamScheduler.ScheduleActionCall(this, IntervalInMilliseconds,
                        TimerScheduledCallType.Once);
                    return context;
                }

                if (reports.ContainsKey(StartServicesExtensionsWellKnownService.Id))
                {
                    context.Do(new DeleteWorkerProcessEntry(StartServicesExtensionsWellKnownService.Id));
                }
            }

            return context;
        }
    }
}
