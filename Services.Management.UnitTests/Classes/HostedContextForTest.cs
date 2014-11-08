namespace Services.Management.UnitTests.Classes
{
    using Chains;
    using Services.Management.Administration.Worker;

    public class HostedContextForTest : Chain<HostedContextForTest>
    {
        public static string ServerProviderType;

        public HostedContextForTest(WorkUnitContext workUnitContext)
        {
            ServerProviderType = workUnitContext.ContextServer.ServerProtocolStack.ServerProvider.GetType().FullName;
        }
    }
}
