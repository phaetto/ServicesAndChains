namespace Services.Management.Administration.Server
{
    using System;
    using System.Collections.Generic;
    using Services.Management.Administration.Worker;

    [Serializable]
    public class WorkUnitReportData
    {
        public DateTime StartedTime;
        public DateTime LastTimeContacted;
        public TimeSpan Uptime;
        public List<Exception> Errors;
        public int ErrorCount;
        public string Log;
        public int LogSizeInCharacters;
        public AdviceState AdviceGiven;
        public WorkUnitState WorkerState;
        public StartWorkerData StartData;
        public ProcessPerformanceData PerformanceData;
    }
}
