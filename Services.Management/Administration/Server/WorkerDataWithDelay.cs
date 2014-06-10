namespace Services.Management.Administration.Server
{
    using System;
    using Chains.Play;
    using Services.Management.Administration.Worker;

    [Serializable]
    public class WorkerDataWithDelay : SerializableSpecification
    {
        public StartWorkerData WorkerData;
        public int DelayInSeconds;

        public override int DataStructureVersionNumber
        {
            get
            {
                return 1;
            }
        }
    }
}
