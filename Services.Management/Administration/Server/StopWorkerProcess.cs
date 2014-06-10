namespace Services.Management.Administration.Server
{
    using Chains;
    using Chains.Play;
    using Chains.Play.Security;

    public sealed class StopWorkerProcess : ReproducibleWithSerializableData<string>,
        IChainableAction<AdministrationContext, AdministrationContext>,
        IAuthorizableAction, IApplicationAuthorizableAction
    {
        public StopWorkerProcess(string data)
            : base(data)
        {
        }

        public AdministrationContext Act(AdministrationContext context)
        {
            if (context.ReportData.ContainsKey(Data))
            {
                context.ReportData[Data].AdviceGiven = AdviceState.Stop;
            }

            return context;
        }

        public string Session { get; set; }

        public string ApiKey { get; set; }
    }
}
