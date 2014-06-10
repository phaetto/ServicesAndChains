namespace Services.Management.Administration.Worker
{
    using System;
    using System.Diagnostics;
    using Chains.Play;

    [Serializable]
    public class ProcessPerformanceData : SerializableSpecification
    {
        public long WorkingSet64;

        public int BasePriority;

        public ProcessPriorityClass PriorityClass;

        public TimeSpan UserProcessorTime;

        public TimeSpan PrivilegedProcessorTime;

        public TimeSpan TotalProcessorTime;

        public long PagedSystemMemorySize64;

        public long PagedMemorySize64;

        public long PeakPagedMemorySize64;

        public long PeakVirtualMemorySize64;

        public long PeakWorkingSet64;

        public override int DataStructureVersionNumber
        {
            get
            {
                return 1;
            }
        }
    }
}
