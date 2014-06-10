namespace Services.Management.Administration.Worker
{
    public interface IWorkerEvents
    {
        void OnStart();

        void OnStop();
    }
}
