namespace Services.Management.UnitTests
{
    using Chains.Play.Web;
    using Chains.UnitTests.Classes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Services.Communication;
    using Services.Management.Administration.Server;
    using Services.Management.Administration.Worker;

    [TestClass]
    public class AdministrationServerTests
    {
        [TestMethod]
        public void ServerHost_WhenAWorkUnitContextConnects_ThenThereIsNoError()
        {
            using (new ServerHost("127.0.0.1", 12005).Do(new EnableAdminServer()))
            {
                using (new WorkUnitContext(
                    new StartWorkerData
                    {
                        ServiceName = "test 1",
                        ContextType = typeof(ContextForTest).FullName,
                        AdminHost = "localhost",
                        AdminPort = 12005
                    }).Do(new StartWorkUnit()))
                {
                }
            }
        }
    }
}
