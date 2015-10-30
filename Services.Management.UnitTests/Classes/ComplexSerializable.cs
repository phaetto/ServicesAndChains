namespace Services.Management.UnitTests.Classes
{
    using System.Collections.Generic;
    using Chains.Play;
    using Services.Management.Administration.Worker;

    public sealed class ComplexSerializable : SerializableSpecification
    {
        public List<StartWorkerData> WorkerData = new List<StartWorkerData>();

        public StartWorkerData[] workerDataArray = new StartWorkerData[10];

        public override int DataStructureVersionNumber => 1;
    }
}
