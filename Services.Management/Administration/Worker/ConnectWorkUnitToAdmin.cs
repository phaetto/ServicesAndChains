namespace Services.Management.Administration.Worker
{
    using Chains;
    using Chains.Play.Web;
    using Services.Communication.Protocol;

    public sealed class ConnectWorkUnitToAdmin : IChainableAction<WorkUnitContext, WorkUnitContext>
    {
        public WorkUnitContext Act(WorkUnitContext context)
        {
            context.AdminServer = new Client(context.WorkerData.AdminHost, context.WorkerData.AdminPort).Do(new OpenConnection());

            return context;
        }
    }
}
