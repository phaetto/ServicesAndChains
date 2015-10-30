namespace Services.Management.Administration.Worker
{
    using System;
    using Chains.Play;

    [Serializable]
    public class ReportProgressData : SerializableSpecification
    {
        public DateTime StartedTime;
        public Exception LastError;
        public string AdditionalLog;
        public AdviceState Advice;
        public StartWorkerData StartData;
        public ProcessPerformanceData PerformanceData = new ProcessPerformanceData();
        public int NextUpdateInSeconds = 5;

        public override int DataStructureVersionNumber => 1;
    }
}
