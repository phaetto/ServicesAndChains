namespace Services.Management.Administration.Server.LastWellKnownConfiguration
{
    using Services.Management.Administration.Worker;

    public sealed class ServiceStartedData
    {
        public int ProcessId;

        public StartWorkerData WorkerData;
    }
}
