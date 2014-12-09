namespace Services.Management.Administration.Server.LastWellKnownConfiguration
{
    using System;
    using Services.Management.Administration.Worker;

    public sealed class ServiceStartedData
    {
        public int ProcessId;

        public StartWorkerData WorkerData;
    }
}
