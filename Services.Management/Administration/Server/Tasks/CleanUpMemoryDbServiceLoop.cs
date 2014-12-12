namespace Services.Management.Administration.Server.Tasks
{
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;
    using Chains;
    using Chains.Play.Web;
    using Services.Communication.Protocol;
    using Services.Management.Administration.Worker;

    public sealed class CleanUpMemoryDbServiceLoop : IChainableAction<AdministrationContext, Task<AdministrationContext>>
    {
        public Task<AdministrationContext> Act(AdministrationContext context)
        {
            var reports = context.Do(new GetReportedData()).Reports;

            try
            {
                using (new Client("127.0.0.1", 51234).Do(new OpenConnection()))
                {
                    context.Do(new StopWorkerProcess(AdministrationContext.ReloadingMemoryDbServiceName));
                    return TaskEx.Delay(1000).ContinueWith(x => context.Do(this)).Unwrap();
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
                return TaskEx.Delay(1000).ContinueWith(x => context.Do(this)).Unwrap();
            }

            return TaskEx.FromResult(context.Do(new DeleteWorkerProcessEntry(AdministrationContext.ReloadingMemoryDbServiceName)));
        }
    }
}
