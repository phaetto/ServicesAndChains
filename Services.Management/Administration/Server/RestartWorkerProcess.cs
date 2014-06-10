namespace Services.Management.Administration.Server
{
    using Chains;
    using Chains.Play;
    using Chains.Play.Security;
    using Services.Management.Administration.Worker;

    public sealed class RestartWorkerProcess : ReproducibleWithSerializableData<string>,
        IChainableAction<AdministrationContext, AdministrationContext>,
        IAuthorizableAction, IApplicationAuthorizableAction
    {
        public RestartWorkerProcess(string data)
            : base(data)
        {
        }

        public AdministrationContext Act(AdministrationContext context)
        {
            if (context.ReportData.ContainsKey(Data))
            {
                if (context.ReportData[Data].WorkerState == WorkUnitState.Running)
                {
                    context.ReportData[Data].AdviceGiven = AdviceState.Restart;

                    context.Do(
                        new StartWorkerProcessWithDelay(
                            new WorkerDataWithDelay
                            {
                                DelayInSeconds = 10,
                                WorkerData = context.ReportData[Data].StartData
                            }));
                }
                else
                {
                    context.Do(new StartWorkerProcess(context.ReportData[Data].StartData));
                }
            }

            return context;
        }

        public string Session { get; set; }

        public string ApiKey { get; set; }
    }
}
