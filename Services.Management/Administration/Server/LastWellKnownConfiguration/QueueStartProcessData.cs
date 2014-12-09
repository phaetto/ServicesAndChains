namespace Services.Management.Administration.Server.LastWellKnownConfiguration
{
    using System.Diagnostics;
    using Chains;
    using Services.Management.Administration.Worker;

    public sealed class QueueStartProcessData : IChainableAction<LastWellKnownConfigurationContext, LastWellKnownConfigurationContext>
    {
        private readonly Process process;

        private readonly StartWorkerData workerData;

        public QueueStartProcessData(StartWorkerData workerData, Process process = null)
        {
            this.process = process;
            this.workerData = workerData;
        }

        public LastWellKnownConfigurationContext Act(LastWellKnownConfigurationContext context)
        {
            context.ServicesThatHaveStartedConcurrentQueue.Enqueue(
                new ServiceStartedData
                {
                    ProcessId = process != null ? process.Id : -1,
                    WorkerData = workerData,
                });

            return context;
        }
    }
}
