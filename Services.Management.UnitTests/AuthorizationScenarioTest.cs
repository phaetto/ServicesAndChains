namespace Services.Management.UnitTests
{
    using System.Collections.Generic;
    using Chains.Play.Modules;
    using Chains.Play.Security.Provider;
    using Chains.Play.Web;
    using Chains.Play.Web.HttpListener;
    using Chains.UnitTests.Classes;
    using Chains.UnitTests.Classes.Security;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Services.Communication.Protocol;
    using Services.Management.Administration.Executioner;
    using Services.Management.Administration.Worker;
    using Services.Management.UnitTests.Classes;

    [TestClass]
    public class AuthorizationScenarioTest
    {
        private const string ValidSession = "1234";

        [TestMethod]
        public void IAuthorizationProvider_WhenProviderIsBeenSupported_ThenApplicationCanLoginAndUseSessionId()
        {
            const int port = 5770;
            const int contextPort = 5772;
            const int httpPort = 5771;

            var adminDataWithModules = new StartWorkerData
                                       {
                                           AdminHost = "127.0.0.1",
                                           AdminPort = port,
                                           Modules = new List<ModuleStartEntry>
                                                     {
                                                         new ModuleStartEntry
                                                         {
                                                             ModuleType = typeof(SecurityProvider).FullName,
                                                             ModuleParameters = new object[]
                                                                                {
                                                                                    httpPort
                                                                                }
                                                         },
                                                         new ModuleStartEntry
                                                         {
                                                             ModuleType = typeof(SecurityModule).FullName,
                                                             ModuleParameters = null
                                                         }
                                                     }
                                       };

            var workerDataWithModules = new StartWorkerData
                                        {
                                            AdminHost = "localhost",
                                            AdminPort = port,
                                            ContextType = typeof(SecuredContextForTest).FullName,
                                            ContextServerHost = "127.0.0.1",
                                            ContextServerPort = contextPort,
                                            Id = "test",
                                            Modules = new List<ModuleStartEntry>
                                                      {
                                                          new ModuleStartEntry
                                                          {
                                                              ModuleType =
                                                                  typeof(SecurityModuleForOnlyAuthorizedActions)
                                                                  .FullName,
                                                              ModuleParameters = null
                                                          }
                                                      }
                                        };

            using (var admin = new WorkerExecutioner(ExecutionMode.AdministrationServer, adminDataWithModules))
            {
                admin.Execute();

                using (var worker = new WorkerExecutioner(ExecutionMode.Worker, workerDataWithModules, processExit: new NoProcessExit()))
                {
                    worker.Execute();

                    using (var client = new Client("localhost", port).Do(new OpenConnection()))
                    {
                        Assert.IsTrue(client.Do(new Send<bool>(new SupportsModule(typeof(IAuthenticationGate).FullName))));

                        var authorizationOptions =
                            client.Do(new Send<ProviderAuthenticationGateData>(new GetAuthenticationGateOptions()));

                        using (var httpServer = new ServerHost(new Client("localhost", httpPort)).Do(new StartHttpServer()))
                        {
                            httpServer.Modules.Add(new HttpLoginProviderForTest(ValidSession, httpServer));

                            Assert.AreEqual(LoginUrlMethod.PostRequest, authorizationOptions.LoginUrlMethod);

                            // After that login on the server that pointed from authorization server
                            var responseResult = HttpRequest.DoRequest(
                                authorizationOptions.LoginUrl,
                                "post",
                                string.Format(
                                    "username={0}&password={1}",
                                    HttpLoginProviderForTest.UserName,
                                    HttpLoginProviderForTest.Password),
                                HttpRequest.FormUrlEncodedContentType);

                            Assert.IsFalse(responseResult.HasError);
                            var resultedSession = responseResult.Response;

                            Assert.AreEqual(ValidSession, resultedSession);

                            // Then use the session to make the request we need
                            using (var workerClient = new Client("localhost", contextPort).Do(new OpenConnection()))
                            {
                                workerClient.Do(
                                    new Send(
                                        new SecuredAuthorizableActionForTest(
                                            new ReproducibleTestData
                                            {
                                                ChangeToValue = "to-value"
                                            })
                                        {
                                            Session = resultedSession
                                        }));
                            }
                        }
                    }

                    Assert.AreEqual("to-value", (worker.WrappedContext as SecuredContextForTest).contextVariable);
                }
            }
        }
    }
}
