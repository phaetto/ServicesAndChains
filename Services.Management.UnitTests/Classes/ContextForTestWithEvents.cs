namespace Services.Management.UnitTests.Classes
{
    using Chains;
    using Services.Management.Administration.Worker;

    public class ContextForTestWithEvents : Chain<ContextForTestWithEvents>, IWorkerEvents
    {
        public const string SuccessfullyStoppedMessage = "Stopped";

        public string contextVariable = null;

        public void OnStart()
        {
        }

        public void OnStop()
        {
            contextVariable = SuccessfullyStoppedMessage;
        }
    }
}
