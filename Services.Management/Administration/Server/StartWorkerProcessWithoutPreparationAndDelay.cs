namespace Services.Management.Administration.Server
{
    using System.Threading;
    using Chains;
    using Chains.Play;
    using Chains.Play.Security;

    public sealed class StartWorkerProcessWithoutPreparationAndDelay : ReproducibleWithData<WorkerDataWithDelay>,
        IChainableAction<AdministrationContext, AdministrationContext>,
        IAuthorizableAction, IApplicationAuthorizableAction
    {
        public StartWorkerProcessWithoutPreparationAndDelay(WorkerDataWithDelay data)
            : base(data)
        {
        }

        public AdministrationContext Act(AdministrationContext context)
        {
            ThreadPool.QueueUserWorkItem(
                x =>
                {
                    Thread.Sleep(Data.DelayInSeconds * 1000);

                    context.Do(new StartWorkerProcessWithoutPreparing(Data.WorkerData, Data.TriesLeft));
                });

            return context;
        }

        public string Session { get; set; }

        public string ApiKey { get; set; }
    }
}