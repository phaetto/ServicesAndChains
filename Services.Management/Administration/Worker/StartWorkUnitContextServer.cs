namespace Services.Management.Administration.Worker
{
    using Chains;
    using Chains.Play.Web;
    using Services.Communication.Protocol;

    public sealed class StartWorkUnitContextServer : IChainableAction<WorkUnitContext, WorkUnitContext>
    {
        public WorkUnitContext Act(WorkUnitContext context)
        {
            if (!string.IsNullOrEmpty(context.WorkerData.ContextServerHost) && context.WorkerData.ContextServerPort > 0)
            {
                var httpPath = context.WorkerData.ContextHttpData?.Path;
                var protocolType = context.WorkerData.ContextHttpData != null ? ProtocolType.Http : ProtocolType.Tcp;
                var threads = context.WorkerData.ContextServerThreads > 1 ? context.WorkerData.ContextServerThreads : 1;

                context.ContextServer =
                    new ServerHost(
                        context.WorkerData.ContextServerHost,
                        context.WorkerData.ContextServerPort,
                        httpPath,
                        threads).Do(new StartListen(protocolType: protocolType));
            }

            return context;
        }
    }
}
