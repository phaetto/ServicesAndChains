namespace Services.Management.UnitTests
{
    using Chains.Play;
    using Chains.Play.Web;
    using Chains.UnitTests.Classes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Services.Communication.Protocol;
    using Services.Management.Administration.Server;
    using Services.Management.Administration.Worker;
    using Services.Management.UnitTests.Classes;

    [TestClass]
    public class WorkUnitTests
    {
        public static AdministrationContext AdministrationContext;

        [ClassInitialize]
        public static void WorkUnitTestsInitialize(TestContext testContext)
        {
            AdministrationContext = new ServerHost("127.0.0.1", 10600).Do(new EnableAdminServer());
        }

        [ClassCleanup]
        public static void WorkUnitTestsCleanup()
        {
            AdministrationContext.Close();
            AdministrationContext.Dispose();
        }

        [TestMethod]
        public void WorkUnitContext_WhenSendingReportAndRequestingReportData_ThenTheDataAreIncludedInTheReport()
        {
            const string lineToLog = "Something to log";

            using (var context = new WorkUnitContext(
                new StartWorkerData
                {
                    Id = "test-1",
                    ServiceName = "test service",
                    ContextType = typeof(ContextForTest).FullName,
                    AdminHost = "localhost",
                    AdminPort = 10600
                }).Do(new ConnectWorkUnitToAdmin()).Do(new StartWorkUnit()))
            {
                context.Log(lineToLog);
                context.CanStop = false;
                context.State = WorkUnitState.Stopping;
                context.ReportToAdmin();
                context.CanStop = true;
            }

            using (var context = new Client("localhost", 10600).Do(new OpenConnection()))
            {
                var result = context.Do(new Send<GetReportedDataReturnData>(new GetReportedData()));

                Assert.IsNotNull(result);
                Assert.IsNotNull(result.Reports);
                Assert.IsNotNull(result.Reports["test-1"]);
                Assert.IsTrue(result.Reports["test-1"].Log.EndsWith(lineToLog));
            }
        }

        [TestMethod]
        public void Chain_WhenObjectIsSerializedToJsonAndDeserialized_ThenTheObjectIsTheSame()
        {
            var data = new StartWorkerData
            {
                Id = "test-1",
                ServiceName = "test service",
                ContextType = typeof(ContextForTest).FullName,
                AdminHost = "localhost",
                AdminPort = 10600,
                Parameters = new object[] { "a", 10, true }
            };

            var serializableData = data.SerializeToJson();

            var deserializedData =
                DeserializableSpecification<StartWorkerData>.DeserializeFromJson(serializableData);

            Assert.AreEqual("test-1", deserializedData.Id);
            Assert.AreEqual(3, deserializedData.Parameters.Length);
            Assert.AreEqual(typeof(string), deserializedData.Parameters[0].GetType());
            Assert.AreEqual(typeof(long), deserializedData.Parameters[1].GetType());
            Assert.AreEqual(typeof(bool), deserializedData.Parameters[2].GetType());
        }

        [TestMethod]
        public void Chain_WhenComplexObjectIsSerializedToJsonAndDeserialized_ThenTheObjectIsTheSame()
        {
            var data = new ComplexSerializable();

            var serializableData = data.SerializeToJson();

            var deserializedData =
                DeserializableSpecification<ComplexSerializable>.DeserializeFromJson(serializableData);

            Assert.AreEqual(data.WorkerDataArray.Length, deserializedData.WorkerDataArray.Length);
        }
    }
}
