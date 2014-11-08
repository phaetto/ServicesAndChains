namespace Services.Management.Administration.Worker
{
    using Chains;
    using Services.Communication.Protocol;

    public sealed class ConnectHostedObject : IChainableAction<WorkUnitContext, WorkUnitContext>
    {
        private readonly object hostedObject;

        private readonly ServerConnectionContext contextServer;

        public ConnectHostedObject(object hostedObject)
        {
            this.hostedObject = hostedObject;
        }

        public WorkUnitContext Act(WorkUnitContext context)
        {
            context.HostedObject = hostedObject;

            context.WorkerControl = hostedObject as IWorkerEvents;

            if (context.WorkerControl != null)
            {
                context.WorkerControl.OnStart();
            }

            return context;
        }
    }
}
