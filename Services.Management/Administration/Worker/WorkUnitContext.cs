namespace Services.Management.Administration.Worker
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using Chains;
    using Services.Communication.Protocol;
    using Services.Management.Administration.Executioner;
    using Services.Management.Administration.Server;

    public sealed class WorkUnitContext : Chain<WorkUnitContext>, IDisposable
    {
        public readonly StartWorkerData WorkerData;
        public readonly string Session;
        public readonly string ApiKey;
        public readonly ReportProgressData ProgressData = new ReportProgressData();
        public readonly DateTime TimeStarted = DateTime.UtcNow;

        private readonly IProcessExit processExit;

        public WorkUnitState State { get; set; }
        public ClientConnectionContext AdminServer { get; set; }
        public ServerConnectionContext ContextServer { get; set; }

        internal Thread ReportThread { get; set; }
        internal object HostedObject { get; set; }
        internal IWorkerEvents WorkerControl { get; set; }

        internal readonly PerformanceCounter CpuCounter = new PerformanceCounter(
            "Process",
            "% Processor Time",
            Process.GetCurrentProcess().ProcessName);

        private bool canStop = true;

        public bool CanStop
        {
            get
            {
                return canStop;
            }
            set
            {
                canStop = value;
            }
        }

        public WorkUnitContext(StartWorkerData workerData, string session = null, string apiKey = null, IProcessExit processExit = null)
        {
            this.WorkerData = workerData;
            this.Session = session;
            this.ApiKey = apiKey;
            this.processExit = processExit;

            CpuCounter.NextValue();
        }

        public void Stop()
        {
            this.ProgressData.Advice = AdviceState.Stop;
            this.State = WorkUnitState.Stopping;
        }

        public void Close()
        {
            Stop();

            try
            {
                ReportToAdmin();
            }
            catch
            {
            }

            if (ReportThread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId)
            {
                ReportThread.Abort();
            }

            if (AdminServer != null)
            {
                AdminServer.Dispose();
                AdminServer = null;
            }

            if (ContextServer != null)
            {
                ContextServer.Dispose();
                ContextServer = null;
            }

            try
            {
                var disposableObject = HostedObject as IDisposable;
                if (disposableObject != null)
                {
                    disposableObject.Dispose();
                }
            }
            catch
            {
            }

            if (processExit != null)
            {
                processExit.Exit();
            }
        }

        public void Dispose()
        {
            try
            {
                Close();
            }
            catch
            {
            }
        }

        public void Log(string message)
        {
            lock (ProgressData)
            {
                LogMessageFormatter(message);
            }
        }

        public void Log(string message, params object[] arguments)
        {
            lock (ProgressData)
            {
                LogMessageFormatter(string.Format(message, arguments));
            }
        }

        public void LogException(Exception exception)
        {
            lock (ProgressData)
            {
                ProgressData.LastError = exception;
            }
        }

        public void LogLine(string message)
        {
            lock (ProgressData)
            {
                LogMessageFormatter(message + Environment.NewLine);
            }
        }

        public void LogLine(string message, params object[] arguments)
        {
            lock (ProgressData)
            {
                LogMessageFormatter(string.Format(message, arguments) + Environment.NewLine);
            }
        }

        private void LogMessageFormatter(string text)
        {
            ProgressData.AdditionalLog =
                $"{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")} (utc) {text}{ProgressData.AdditionalLog}";
        }

        internal void ReportToAdminThread()
        {
            var reportUpdateInMilliseconds = WorkerData.ReportUpdateIntervalInSeconds * 1000;

            while (State == WorkUnitState.Running || !CanStop)
            {
                try
                {
                    ReportToAdmin();
                }
                catch
                {
                    if (State == WorkUnitState.Running)
                    {
                        Thread.Sleep(10000);
                        try
                        {
                            AdminServer.Do(new RetryConnection());
                        }
                        catch
                        {
                        }
                    }
                }

                Thread.Sleep(reportUpdateInMilliseconds);
            }

            if (WorkerControl != null)
            {
                try
                {
                    WorkerControl.OnStop();
                }
                catch (Exception ex)
                {
                    LogException(ex);
                }
            }

            Dispose();
        }

        public void ReportToAdmin()
        {
            ReportProgressReturnData reportProgress;
            lock (ProgressData)
            {
                ProgressData.StartData = WorkerData;
                ProgressData.StartedTime = TimeStarted;

                var myProcess = Process.GetCurrentProcess();
                ProgressData.PerformanceData.WorkingSet64 = myProcess.WorkingSet64;
                ProgressData.PerformanceData.BasePriority = myProcess.BasePriority;
                ProgressData.PerformanceData.PriorityClass = myProcess.PriorityClass;
                ProgressData.PerformanceData.UserProcessorTime = myProcess.UserProcessorTime;
                ProgressData.PerformanceData.PrivilegedProcessorTime = myProcess.PrivilegedProcessorTime;
                ProgressData.PerformanceData.TotalProcessorTime = myProcess.TotalProcessorTime;
                ProgressData.PerformanceData.PagedSystemMemorySize64 = myProcess.PagedSystemMemorySize64;
                ProgressData.PerformanceData.PagedMemorySize64 = myProcess.PagedMemorySize64;
                ProgressData.PerformanceData.PeakPagedMemorySize64 = myProcess.PeakPagedMemorySize64;
                ProgressData.PerformanceData.PeakVirtualMemorySize64 = myProcess.PeakVirtualMemorySize64;
                ProgressData.PerformanceData.PeakWorkingSet64 = myProcess.PeakWorkingSet64;
                ProgressData.PerformanceData.CpuPercentage = CpuCounter.NextValue();
                if (ProgressData.PerformanceData.DiskUsageInBytes == 0)
                {
                    ProgressData.PerformanceData.DiskUsageInBytes =
                        PrepareWorkerProcessFiles.MeasureFilesDiskSize(AppDomain.CurrentDomain.BaseDirectory);
                }

                reportProgress = AdminServer.Do(new Send<ReportProgressReturnData>(new ReportProgress(ProgressData), timesToRetry: 0));
                ProgressData.AdditionalLog = string.Empty;
                ProgressData.LastError = null;
            }

            if (reportProgress.Advice == AdviceState.Stop || reportProgress.Advice == AdviceState.Restart)
            {
                State = WorkUnitState.Stopping;
            }
        }
    }
}
