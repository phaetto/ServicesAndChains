namespace Services.Management.Administration.Worker
{
    using System;
    using System.Threading;
    using Chains;
    using Chains.Play.Web;
    using Services.Communication.Protocol;

    public sealed class StartWorkUnit : IChainableAction<WorkUnitContext, WorkUnitContext>
    {
        public WorkUnitContext Act(WorkUnitContext context)
        {   
            context.AdminServer = new Client(context.WorkerData.AdminHost, context.WorkerData.AdminPort).Do(new OpenConnection());

            context.State = WorkUnitState.Running;

            context.TimeStarted = DateTime.UtcNow;

            context.ReportThread = new Thread(context.ReportToAdminThread);
            context.ReportThread.Start();

            return context;
        }
    }
}
