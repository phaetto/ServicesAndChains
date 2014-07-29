namespace Services.Management.Administration.Server
{
    using System.Threading;
    using Chains;
    using Chains.Play;
    using Chains.Play.Security;
    using Services.Management.Administration.Worker;

    public sealed class RestartWorkerProcess : ReproducibleWithSerializableData<string>,
        IChainableAction<AdministrationContext, AdministrationContext>,
        IAuthorizableAction, IApplicationAuthorizableAction
    {
        private const int WaitDelayInMilliseconds = 1000;

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

                    while (context.ReportData[Data].WorkerState == WorkUnitState.Running)
                    {
                        Thread.Sleep(WaitDelayInMilliseconds);
                    }
                }

                context.Do(new StartWorkerProcess(context.ReportData[Data].StartData));
            }

            return context;
        }

        public string Session { get; set; }

        public string ApiKey { get; set; }
    }
}
