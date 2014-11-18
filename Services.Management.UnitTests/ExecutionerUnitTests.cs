namespace Services.Management.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using Chains.Play.Web;
    using Chains.Play.Web.HttpListener;
    using Chains.UnitTests.Classes;
    using Chains.UnitTests.Classes.Security;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Services.Communication.Protocol;
    using Services.Communication.Tcp.Servers;
    using Services.Management.Administration.Executioner;
    using Services.Management.Administration.Server;
    using Services.Management.Administration.Worker;
    using Services.Management.UnitTests.Classes;

    [TestClass]
    public class ExecutionerUnitTests
    {
        public static AdministrationContext AdministrationContext;

        [ClassInitialize]
        public static void WorkUnitTestsInitialize(TestContext testContext)
        {
            AdministrationContext = new ServerHost("127.0.0.1", 10500).Do(new EnableAdminServer());
        }

        [ClassCleanup]
        public static void WorkUnitTestsCleanup()
        {
            AdministrationContext.Close();
            AdministrationContext.Dispose();
        }

        [TestMethod]
        public void WorkerExecutioner_WhenWorkerIsExposedWithTcp_ThenClassCanAccessServer()
        {
            var workerDataWithoutModules = new StartWorkerData
            {
                AdminHost = AdministrationContext.Parent.Parent.Hostname,
                AdminPort = AdministrationContext.Parent.Parent.Port,
                ContextType = typeof(HostedContextForTest).FullName,
                ContextServerHost = "127.0.0.1",
                ContextServerPort = 10501,
                Id = "test",
            };

            using (var executioner = new WorkerExecutioner(ExecutionMode.Worker, workerDataWithoutModules, processExit: new NoProcessExit()))
            {
                executioner.Execute();
            }

            Assert.AreEqual(HostedContextForTest.ServerProviderType, typeof(TcpServer).FullName);
        }

        [TestMethod]
        public void WorkerExecutioner_WhenWorkerIsExposedWithHttp_ThenClassCanAccessServer()
        {
            var workerDataWithoutModules = new StartWorkerData
            {
                AdminHost = AdministrationContext.Parent.Parent.Hostname,
                AdminPort = AdministrationContext.Parent.Parent.Port,
                ContextType = typeof(HostedContextForTest).FullName,
                ContextServerHost = "127.0.0.1",
                ContextServerPort = 10501,
                ContextHttpData = new StartWorkerHttpData
                                  {
                                      Path = "/"
                                  },
                Id = "test",
            };

            using (var executioner = new WorkerExecutioner(ExecutionMode.Worker, workerDataWithoutModules, processExit: new NoProcessExit()))
            {
                executioner.Execute();
            }

            Assert.AreEqual(HostedContextForTest.ServerProviderType, typeof(HttpServer).FullName);
        }

        [TestMethod]
        public void WorkerExecutioner_WhenObjectCanGetInjectedObjects_ThenClassCanAccessInjectedInstances()
        {
            var workerDataWithoutModules = new StartWorkerData
            {
                AdminHost = AdministrationContext.Parent.Parent.Hostname,
                AdminPort = AdministrationContext.Parent.Parent.Port,
                ContextType = typeof(ContextWithInjection).FullName,
                Id = "test",
            };

            using (var executioner = new WorkerExecutioner(ExecutionMode.Worker, workerDataWithoutModules, processExit: new NoProcessExit()))
            {
                executioner.Execute();

                var contextWithInjection = executioner.WrappedContext as ContextWithInjection;
                Assert.IsNotNull(contextWithInjection);

                Assert.IsNotNull(contextWithInjection.WorkUnitContext);
            }
        }

        [TestMethod]
        public void WorkerExecutioner_WhenObjectIsBeenCreated_ThenItHasAccessToAdminConnection()
        {
            var workerDataWithoutModules = new StartWorkerData
            {
                AdminHost = AdministrationContext.Parent.Parent.Hostname,
                AdminPort = AdministrationContext.Parent.Parent.Port,
                ContextType = typeof(ContextWithInjection).FullName,
                Id = "test",
            };

            using (var executioner = new WorkerExecutioner(ExecutionMode.Worker, workerDataWithoutModules, processExit: new NoProcessExit()))
            {
                executioner.Execute();

                var contextWithInjection = executioner.WrappedContext as ContextWithInjection;
                Assert.IsNotNull(contextWithInjection);

                Assert.IsNotNull(contextWithInjection.WorkUnitContext);
                Assert.IsNotNull(contextWithInjection.WorkUnitContext.AdminServer);
            }
        }

        [TestMethod]
        public void WorkerExecutioner_WhenWorkerIncludesSecurityModule_ThenModuleIsBeenEnforcedAndExceptionIsThrown()
        {
            var workerDataWithModules = new StartWorkerData
                                        {
                                            AdminHost = AdministrationContext.Parent.Parent.Hostname,
                                            AdminPort = AdministrationContext.Parent.Parent.Port,
                                            ContextType = typeof(ContextForTestWithModules).FullName,
                                            ContextServerHost = "127.0.0.1",
                                            ContextServerPort = 10501,
                                            Id = "test",
                                            Modules = new List<ModuleStartEntry>
                                                      {
                                                          new ModuleStartEntry
                                                          {
                                                              ModuleType = typeof(SecurityModule).FullName,
                                                              ModuleParameters = null
                                                          }
                                                      }
                                        };

            using (var executioner = new WorkerExecutioner(ExecutionMode.Worker, workerDataWithModules, processExit: new NoProcessExit()))
            {
                Exception exception = null;

                executioner.Execute();

                try
                {
                    using (var context = new Client("localhost", 10501).Do(new OpenConnection()))
                    {
                        context.Do(new Send(new SecuredAuthorizableActionForTest(new ReproducibleTestData())));
                    }
                }
                catch (Exception ex)
                {
                    exception = ex;
                }

                Assert.IsNotNull(exception);
                Assert.IsInstanceOfType(exception, typeof(SecurityException));
            }
        }

        [TestMethod]
        public void WorkerExecutioner_WhenWorkerIsHostedThroughHttpAndSecurityModule_ThenModuleIsBeenEnforcedAndExceptionIsThrown()
        {
            var workerDataWithModules = new StartWorkerData
            {
                AdminHost = AdministrationContext.Parent.Parent.Hostname,
                AdminPort = AdministrationContext.Parent.Parent.Port,
                ContextType = typeof(ContextForTestWithModules).FullName,
                ContextServerHost = "127.0.0.1",
                ContextServerPort = 10501,
                Id = "test",
                ContextHttpData = new StartWorkerHttpData
                                  {
                                      Path = "/awesome-path"
                                  },
                Modules = new List<ModuleStartEntry>
                                                      {
                                                          new ModuleStartEntry
                                                          {
                                                              ModuleType = typeof(SecurityModule).FullName,
                                                              ModuleParameters = null
                                                          }
                                                      }
            };

            using (var executioner = new WorkerExecutioner(ExecutionMode.Worker, workerDataWithModules, processExit: new NoProcessExit()))
            {
                Exception exception = null;

                executioner.Execute();

                try
                {
                    using (var context = new Client("localhost", 10501, "/awesome-path").Do(new OpenConnection(protocolType: ProtocolType.Http)))
                    {
                        context.Do(new Send(new SecuredAuthorizableActionForTest(new ReproducibleTestData())));
                    }
                }
                catch (Exception ex)
                {
                    exception = ex;
                }

                Assert.IsNotNull(exception);
                Assert.IsInstanceOfType(exception, typeof(SecurityException));
            }
        }

        [TestMethod]
        public void WorkerExecutioner_WhenWorkerIncludesSecurityModuleAndActionIsNotSecure_ThenModuleIsBeenEnforcedAndExceptionIsThrown()
        {
            var workerDataWithModules = new StartWorkerData
            {
                AdminHost = AdministrationContext.Parent.Parent.Hostname,
                AdminPort = AdministrationContext.Parent.Parent.Port,
                ContextType = typeof(ContextForTestWithModules).FullName,
                ContextServerHost = "127.0.0.1",
                ContextServerPort = 10501,
                Id = "test",
                Modules = new List<ModuleStartEntry>
                                                      {
                                                          new ModuleStartEntry
                                                          {
                                                              ModuleType = typeof(SecurityModuleForOnlyAuthorizedActions).FullName,
                                                              ModuleParameters = null
                                                          }
                                                      }
            };

            using (var executioner = new WorkerExecutioner(ExecutionMode.Worker, workerDataWithModules, processExit: new NoProcessExit()))
            {
                Exception exception = null;

                executioner.Execute();

                try
                {
                    using (var context = new Client("localhost", 10501).Do(new OpenConnection()))
                    {
                        context.Do(new Send(new ReproducibleTestAction(new ReproducibleTestData())));
                    }
                }
                catch (Exception ex)
                {
                    exception = ex;
                }

                Assert.IsNotNull(exception);
                Assert.IsInstanceOfType(exception, typeof(InvalidOperationException));
            }
        }

        [TestMethod]
        public void WorkerExecutioner_WhenWorkerDoesNotIncludeASecurityModule_ThenModuleIsNotBeenEnforced()
        {
            var workerDataWithoutModules = new StartWorkerData
            {
                AdminHost = AdministrationContext.Parent.Parent.Hostname,
                AdminPort = AdministrationContext.Parent.Parent.Port,
                ContextType = typeof(ContextForTest).FullName,
                ContextServerHost = "127.0.0.1",
                ContextServerPort = 10501,
                Id = "test",
            };

            using (var executioner = new WorkerExecutioner(ExecutionMode.Worker, workerDataWithoutModules, processExit: new NoProcessExit()))
            {
                executioner.Execute();

                using (var context = new Client("localhost", 10501).Do(new OpenConnection()))
                {
                    context.Do(new Send(new ReproducibleTestAction(new ReproducibleTestData())));
                }
            }
        }
    }
}
