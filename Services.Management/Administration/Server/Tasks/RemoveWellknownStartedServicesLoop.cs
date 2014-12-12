namespace Services.Management.Administration.Server.Tasks
{
    using System.IO;
    using System.Threading.Tasks;
    using Chains;
    using Services.Management.Administration.Worker;

    public sealed class RemoveWellKnownStartedServicesLoop : IChainableAction<AdministrationContext, Task<AdministrationContext>>
    {
        public Task<AdministrationContext> Act(AdministrationContext context)
        {
            var reports = context.ReportData;

            if (!reports.ContainsKey(StartServicesExtensionsWellKnownService.Id) ||
                reports[StartServicesExtensionsWellKnownService.Id].WorkerState != WorkUnitState.Stopping)
            {
                return TaskEx.Delay(1000).ContinueWith(x => context.Do(this)).Unwrap();
            }

            if (reports.ContainsKey(StartServicesExtensionsWellKnownService.Id))
            {
                try
                {
                    context.Do(new DeleteWorkerProcessEntry(StartServicesExtensionsWellKnownService.Id));

                }
                catch (IOException)
                {
                    // Might taken from autoservice, and does not exists in the filesystem
                }
            }

            return TaskEx.FromResult(context);
        }
    }
}
