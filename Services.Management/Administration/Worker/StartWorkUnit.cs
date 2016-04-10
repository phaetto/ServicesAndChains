namespace Services.Management.Administration.Worker
{
    using System.Threading;
    using Chains;

    public sealed class StartWorkUnit : IChainableAction<WorkUnitContext, WorkUnitContext>
    {
        public WorkUnitContext Act(WorkUnitContext context)
        {   
            context.State = WorkUnitState.Running;

            context.ReportThread = new Thread(context.ReportToAdminThread);
            context.ReportThread.Start();

            return context;
        }
    }
}
