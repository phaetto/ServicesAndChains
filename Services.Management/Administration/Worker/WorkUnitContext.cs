namespace Services.Management.Administration.Worker
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using Chains;
    using Services.Communication.Protocol;

    public sealed class WorkUnitContext : Chain<WorkUnitContext>, IDisposable
    {
        private bool canStop = true;

        public readonly StartWorkerData WorkerData;

        public readonly string Session;
        public readonly string ApiKey;
        public DateTime TimeStarted { get; set; }
        public WorkUnitState State { get; set; }
        public ClientConnectionContext AdminServer { get; set; }
        public ReportProgressData ProgressData { get; set; }
        public IWorkerEvents WorkerControl { get; set; }

        internal Thread reportThread { get; set; }

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

        public WorkUnitContext(StartWorkerData workerData, string session = null, string apiKey = null)
        {
            this.WorkerData = workerData;
            this.Session = session;
            this.ApiKey = apiKey;
        }

        public void Stop()
        {
            this.ProgressData.Advice = AdviceState.Stop;
            this.State = WorkUnitState.Stopping;
        }

        public void Close()
        {
            Stop();

            if (reportThread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId)
            {
                reportThread.Abort();
            }

            if (AdminServer != null)
                AdminServer.Close();
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
            ProgressData.AdditionalLog = string.Format("{0} (utc) {1}{2}", DateTime.UtcNow.ToString("yyyy-MM-dd hh:mm:ss"), text, ProgressData.AdditionalLog);
        }

        internal void ReportToAdminThread()
        {
            var repotUpdateInMilliseconds = WorkerData.ReportUpdateIntervalInSeconds * 1000;

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

                Thread.Sleep(repotUpdateInMilliseconds);
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
                    ReportToAdmin();
                }
            }

            Dispose();
            Process.GetCurrentProcess().Kill();
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
