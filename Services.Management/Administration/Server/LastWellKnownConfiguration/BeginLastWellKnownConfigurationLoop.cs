namespace Services.Management.Administration.Server.LastWellKnownConfiguration
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Chains;
    using Services.Management.Administration.Worker;

    public sealed class BeginLastWellKnownConfigurationLoop : IChainableAction<LastWellKnownConfigurationContext, Task<LastWellKnownConfigurationContext>>
    {
        public Task<LastWellKnownConfigurationContext> Act(LastWellKnownConfigurationContext context)
        {
            string serviceToProcess;
            if (context.ServicesToCreateSnapshotConcurrentQueue.TryDequeue(out serviceToProcess))
            {
                context.Do(new CreateLastWellKnownConfigurationSnapshot(serviceToProcess));

                return TaskEx.Delay(100).ContinueWith(x => context.Do(this)).Unwrap();
            }

            ServiceStartedData serviceStartedData;
            if (context.ServicesThatHaveStartedConcurrentQueue.TryDequeue(out serviceStartedData))
            {
                try
                {
                    Process.GetProcessById(serviceStartedData.ProcessId);

                    if (context.Parent.ReportData[serviceStartedData.WorkerData.Id] == null
                        || context.Parent.ReportData[serviceStartedData.WorkerData.Id].WorkerState
                        != WorkUnitState.Running)
                    {
                        // Wait
                        context.ServicesThatHaveStartedConcurrentQueue.Enqueue(serviceStartedData);
                    }
                }
                catch (ArgumentException)
                {
                    // Process do not exists, that indicates failure - restore and start another service
                    context.Do(new RestoreLastKnownConfiguration(serviceStartedData.WorkerData));
                    context.Parent.Do(new StartWorkerProcessWithoutPreparing(serviceStartedData.WorkerData, 10));
                }

                return TaskEx.Delay(100).ContinueWith(x => context.Do(this)).Unwrap();
            }

            return TaskEx.Delay(1000).ContinueWith(x => context.Do(this)).Unwrap();
        }
    }
}
