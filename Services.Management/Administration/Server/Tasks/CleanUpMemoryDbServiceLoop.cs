namespace Services.Management.Administration.Server.Tasks
{
    using System.Net.Sockets;
    using Chains;
    using Chains.Play.Streams.Timer;
    using Chains.Play.Web;
    using Services.Communication.Protocol;
    using Services.Management.Administration.Worker;

    public sealed class CleanUpMemoryDbServiceLoop : IChainableAction<AdministrationContext, AdministrationContext>
    {
        public const int IntervalInMilliseconds = 1000;

        public AdministrationContext Act(AdministrationContext context)
        {
            var reports = context.Do(new GetReportedData()).Reports;

            try
            {
                using (new Client("127.0.0.1", 51234).Do(new OpenConnection()))
                {
                    context.Do(new StopWorkerProcess(AdministrationContext.ReloadingMemoryDbServiceName));
                    context.TimerStreamScheduler.ScheduleActionCall(this, IntervalInMilliseconds, TimerScheduledCallType.Once);
                    return context;
                }
            }
            catch (SocketException)
            {
                // Server is down
            }

            // Wait until the process is considered stopped
            if (reports.ContainsKey(AdministrationContext.ReloadingMemoryDbServiceName)
                    && reports[AdministrationContext.ReloadingMemoryDbServiceName].WorkerState != WorkUnitState.Stopping)
            {
                context.TimerStreamScheduler.ScheduleActionCall(this, IntervalInMilliseconds, TimerScheduledCallType.Once);
                return context;
            }

            return context.Do(new DeleteWorkerProcessEntry(AdministrationContext.ReloadingMemoryDbServiceName));
        }
    }
}
