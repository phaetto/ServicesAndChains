namespace Services.Management.Administration.Worker
{
    using System;

    [Serializable]
    public enum WorkUnitState
    {
        Stopping,
        Running,
        Abandoned,
        Updating,
        Restarting
    }
}
