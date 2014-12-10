namespace Services.Management.Administration.Server
{
    using System;
    using Chains;
    using Chains.Play;
    using Chains.Play.Security;
    using Services.Management.Administration.Server.LastWellKnownConfiguration;
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
            try
            {
                Data = context.Do(new PrepareWorkerProcessFiles(Data));

                context.Do(new StartWorkerProcessWithoutPreparing(Data, triesLeft));
            }
            catch (Exception ex)
            {
                if (triesLeft > 0)
                {
                    context.Do(
                        new StartWorkerProcessWithDelay(
                            new WorkerDataWithDelay
                            {
                                DelayInSeconds = 10,
                                WorkerData = Data,
                                TriesLeft = triesLeft - 1
                            }));
                }
                else
                {
                    context.LastWellKnownConfigurationContext.Do(new QueueStartProcessData(Data));
                }

                context.LogException(ex);
            }

            return context;
        }

        public string Session { get; set; }

        public string ApiKey { get; set; }
    }
}
