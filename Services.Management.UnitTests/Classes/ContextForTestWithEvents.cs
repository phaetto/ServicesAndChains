namespace Services.Management.UnitTests.Classes
{
    using Chains;
    using Services.Management.Administration.Worker;

    public class ContextForTestWithEvents : Chain<ContextForTestWithEvents>, IWorkerEvents
    {
        public const string SuccessfullyStoppedMessage = "Stopped";

        public string ContextVariable = null;

        public void OnStart()
        {
        }

        public void OnStop()
        {
            ContextVariable = SuccessfullyStoppedMessage;
        }
    }
}
