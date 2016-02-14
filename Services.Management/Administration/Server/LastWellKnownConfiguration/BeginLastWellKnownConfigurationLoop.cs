namespace Services.Management.Administration.Server.LastWellKnownConfiguration
{
    using System;
    using System.Diagnostics;
    using Chains;
    using Chains.Play.Streams.Timer;
    using Services.Management.Administration.Worker;

    public sealed class BeginLastWellKnownConfigurationLoop : IChainableAction<LastWellKnownConfigurationContext, LastWellKnownConfigurationContext>
    {
        public const int NoWorkTodoIntervalInMilliseconds = 5000;
        public const int IntervalInMilliseconds = 1000;

        public LastWellKnownConfigurationContext Act(LastWellKnownConfigurationContext context)
        {
            string serviceIdToProcess;
            if (context.ServicesToCreateSnapshotConcurrentQueue.TryDequeue(out serviceIdToProcess))
            {
                var workerData = context.Parent.ReportData[serviceIdToProcess];

                if (workerData == null || workerData.WorkerState != WorkUnitState.Running)
                {
                    // Wait until the service gets running, or the process fails.
                    context.ServicesToCreateSnapshotConcurrentQueue.Enqueue(serviceIdToProcess);
                }
                else
                {
                    // Service must be up and contacted recently
                    // Must have a respectable uptime
                    var minimumTimeInSeconds = workerData.StartData.ReportUpdateIntervalInSeconds * 6;
                    if (workerData.WorkerState == WorkUnitState.Running
                        && workerData.Uptime.TotalSeconds > minimumTimeInSeconds)
                    {
                        context.Do(new CreateLastWellKnownConfigurationSnapshot(serviceIdToProcess));
                    }
                    else
                    {
                        context.ServicesToCreateSnapshotConcurrentQueue.Enqueue(serviceIdToProcess);
                    }
                }
            }

            ServiceStartedData serviceStartedData;
            if (context.ServicesThatHaveStartedConcurrentQueue.TryDequeue(out serviceStartedData))
            {
                try
                {
                    Process.GetProcessById(serviceStartedData.ProcessId);

                    if (!context.Parent.ReportData.ContainsKey(serviceStartedData.WorkerData.Id))
                    {
                        context.ServicesThatHaveStartedConcurrentQueue.Enqueue(serviceStartedData);
                    }
                    else
                    {
                        var workerData = context.Parent.ReportData[serviceStartedData.WorkerData.Id];

                        if (workerData.WorkerState != WorkUnitState.Running)
                        {
                            // Wait until the service gets running, or the process fails.
                            context.ServicesThatHaveStartedConcurrentQueue.Enqueue(serviceStartedData);
                        }
                        else
                        {
                            // Service must be up and contacted recently
                            // Must have a respectable uptime
                            var minimumTimeInSeconds = workerData.StartData.ReportUpdateIntervalInSeconds * 6;
                            if (workerData.Uptime.TotalSeconds > minimumTimeInSeconds)
                            {
                                // Working fine
                            }
                            else
                            {
                                context.ServicesThatHaveStartedConcurrentQueue.Enqueue(serviceStartedData);
                            }
                        }
                    }
                }
                catch (ArgumentException)
                {
                    try
                    {
                        // Process do not exists, that indicates failure - restore and start another service
                        context.Do(new RestoreLastKnownConfiguration(serviceStartedData.WorkerData));
                        context.Parent.Do(new StartWorkerProcessWithoutPreparing(serviceStartedData.WorkerData, 10));
                    }
                    catch (InvalidOperationException exception)
                    {
                        // Could not be revived - o lwkc found
                        context.Parent.LogException(exception);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // That means that the process might runs with another id now. Requeue
                        context.ServicesThatHaveStartedConcurrentQueue.Enqueue(serviceStartedData);
                    }
                }
            }

            if (serviceIdToProcess == null && serviceStartedData == null)
            {
                context.TimerStreamScheduler.ScheduleActionCall(this, NoWorkTodoIntervalInMilliseconds, TimerScheduledCallType.Once);
                return context;
            }

            context.TimerStreamScheduler.ScheduleActionCall(this, IntervalInMilliseconds, TimerScheduledCallType.Once);
            return context;
        }
    }
}
