namespace Services.Management.Administration.Server
{
    using Chains;
    using Chains.Play;
    using Chains.Play.Security;
    using Services.Management.Administration.Worker;

    public sealed class StartWorkerProcess : ReproducibleWithData<StartWorkerData>,
        IChainableAction<AdministrationContext, AdministrationContext>,
        IAuthorizableAction, IApplicationAuthorizableAction
    {
        private readonly int triesLeft;

        public StartWorkerProcess(StartWorkerData data)
            : this(data, 10)
        {
        }

        public StartWorkerProcess(StartWorkerData data, int triesLeft)
            : base(data)
        {
            this.triesLeft = triesLeft;
        }

        public AdministrationContext Act(AdministrationContext context)
        {
            Data = context.Do(new PrepareWorkerProcessFiles(Data));

            return context.Do(new StartWorkerProcessWithoutPreparing(Data, triesLeft));
        }

        public string Session { get; set; }

        public string ApiKey { get; set; }
    }
}
