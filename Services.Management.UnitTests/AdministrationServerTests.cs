﻿namespace Services.Management.UnitTests
{
    using System.IO;
    using Chains.Play.Web;
    using Chains.UnitTests.Classes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Services.Communication.Protocol;
    using Services.Management.Administration.Executioner;
    using Services.Management.Administration.Server;
    using Services.Management.Administration.Update;
    using Services.Management.Administration.Worker;
    using Services.Management.UnitTests.Classes;

    [TestClass]
    public class AdministrationServerTests
    {
        private const string ServiceId = "test 1";

        [TestMethod]
        public void ServerHost_WhenAnAdminLoads_ThenDirectoriesAreRelativeToEachOther()
        {
            using (var admin = new ServerHost("127.0.0.1", 12005).Do(new EnableAdminServer()))
            {
                Assert.IsFalse(admin.DataFolder.IndexOf("..") > -1);
                Assert.IsTrue(admin.DataFolder.EndsWith(AdministrationContext.DataFolderName + Path.DirectorySeparatorChar));
                Assert.IsTrue(admin.ServicesFolder.EndsWith(AdministrationContext.ServiceFolderName + Path.DirectorySeparatorChar));
                Assert.IsTrue(admin.RepositoryFolder.EndsWith(AdministrationContext.RepositoryFolderName + Path.DirectorySeparatorChar));

                Assert.AreNotEqual(
                    Directory.GetParent(admin.DataFolder).Parent.FullName,
                    Directory.GetParent(admin.ServicesFolder).Parent.FullName);

                Assert.AreNotEqual(
                    Directory.GetParent(admin.DataFolder).Parent.FullName,
                    Directory.GetParent(admin.RepositoryFolder).Parent.FullName);

                Assert.AreEqual(
                    Directory.GetParent(admin.ServicesFolder).Parent.FullName,
                    Directory.GetParent(admin.RepositoryFolder).Parent.FullName);

                Assert.IsTrue(Directory.Exists(admin.RepositoryFolder));
                Assert.IsTrue(Directory.Exists(admin.ServicesFolder));
                Assert.IsTrue(Directory.Exists(admin.DataFolder));
            }
        }

        [TestMethod]
        public void ServerHost_WhenAWorkUnitContextConnects_ThenThereIsNoError()
        {
            using (new ServerHost("127.0.0.1", 12005).Do(new EnableAdminServer()))
            {
                using (new WorkUnitContext(
                    new StartWorkerData
                    {
                        ServiceName = ServiceId,
                        ContextType = typeof(ContextForTestWithEvents).FullName,
                        AdminHost = "localhost",
                        AdminPort = 12005
                    }).Do(new StartWorkUnit()))
                {
                }
            }
        }

        [TestMethod]
        public void ServerHost_WhenAWorkUnitIsStopped_ThenIsStoppedGracefully()
        {
            using (new ServerHost("127.0.0.1", 12006).Do(new EnableAdminServer()))
            {
                var workerDataWithModules = new StartWorkerData
                                            {
                                                AdminHost = "127.0.0.1",
                                                AdminPort = 12006,
                                                ContextType = typeof(ContextForTestWithModules).FullName,
                                                Id = ServiceId,
                                                ServiceName = ServiceId,
                                                ReportUpdateIntervalInSeconds = 1,
                                            };

                using (var executioner = new WorkerExecutioner(ExecutionMode.Worker, workerDataWithModules, processExit: new NoProcessExit()))
                {
                    executioner.Execute();

                    using (var clientContextToAdmin = new Client("127.0.0.1", 12006).Do(new OpenConnection()))
                    {
                        clientContextToAdmin
                            .Do(new WaitUntilServiceIsUp(ServiceId, 10))
                            .Do(new Send(new StopWorkerProcess(ServiceId)))
                            .Do(new WaitUntilServiceIsStopped(ServiceId, 10));
                    }

                    Assert.AreEqual(WorkUnitState.Stopping, executioner.WorkUnitContext.State);
                }
            }
        }

        [TestMethod]
        public void ServerHost_WhenAWorkUnitIsStoppedAndSupportsEvents_ThenIsStoppedGracefullyAndEventsAreCalled()
        {
            using (new ServerHost("127.0.0.1", 12007).Do(new EnableAdminServer()))
            {
                var workerDataWithModules = new StartWorkerData
                                            {
                                                AdminHost = "127.0.0.1",
                                                AdminPort = 12007,
                                                ContextType = typeof(ContextForTestWithEvents).FullName,
                                                Id = ServiceId,
                                                ServiceName = ServiceId,
                                                ReportUpdateIntervalInSeconds = 1,
                                            };

                using (var executioner = new WorkerExecutioner(ExecutionMode.Worker, workerDataWithModules, processExit: new NoProcessExit()))
                {
                    executioner.Execute();

                    using (var clientContextToAdmin = new Client("127.0.0.1", 12007).Do(new OpenConnection()))
                    {
                        clientContextToAdmin
                            .Do(new WaitUntilServiceIsUp(ServiceId, 10))
                            .Do(new Send(new StopWorkerProcess(ServiceId)))
                            .Do(new WaitUntilServiceIsStopped(ServiceId, 10));
                    }

                    var result = executioner.WrappedContext as ContextForTestWithEvents;

                    Assert.AreEqual(WorkUnitState.Stopping, executioner.WorkUnitContext.State);
                    Assert.AreEqual(ContextForTestWithEvents.SuccessfullyStoppedMessage, result.ContextVariable);
                }
            }
        }

        [TestMethod]
        public void ServerHost_WhenAWorkUnitIsStoppedAndSupportsIDisposable_ThenIsStoppedGracefullyAndDisposed()
        {
            using (new ServerHost("127.0.0.1", 12008).Do(new EnableAdminServer()))
            {
                var workerDataWithModules = new StartWorkerData
                                            {
                                                AdminHost = "127.0.0.1",
                                                AdminPort = 12008,
                                                ContextType = typeof(DisposableContextForTest).FullName,
                                                Id = ServiceId,
                                                ServiceName = ServiceId,
                                                ReportUpdateIntervalInSeconds = 1,
                                            };

                using (var executioner = new WorkerExecutioner(ExecutionMode.Worker, workerDataWithModules, processExit: new NoProcessExit()))
                {
                    executioner.Execute();

                    using (var clientContextToAdmin = new Client("127.0.0.1", 12008).Do(new OpenConnection()))
                    {
                        clientContextToAdmin
                            .Do(new WaitUntilServiceIsUp(ServiceId, 10))
                            .Do(new Send(new StopWorkerProcess(ServiceId)))
                            .Do(new WaitUntilServiceIsStopped(ServiceId, 10));
                    }

                    var result = executioner.WrappedContext as DisposableContextForTest;

                    Assert.AreEqual(WorkUnitState.Stopping, executioner.WorkUnitContext.State);
                    Assert.AreEqual(DisposableContextForTest.SuccessfullyStoppedMessage, result.ContextVariable);
                }
            }
        }
    }
}
