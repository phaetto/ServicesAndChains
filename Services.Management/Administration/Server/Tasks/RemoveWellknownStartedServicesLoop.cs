namespace Services.Management.Administration.Server.Tasks
{
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
                return TaskEx.DelayWithCarbageCollection(5000).ContinueWith(x => context.Do(this)).Unwrap();
            }

            if (reports.ContainsKey(StartServicesExtensionsWellKnownService.Id))
            {
                context.Do(new DeleteWorkerProcessEntry(StartServicesExtensionsWellKnownService.Id));
            }

            return TaskEx.FromResult(context);
        }
    }
}
