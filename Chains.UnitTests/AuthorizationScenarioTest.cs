namespace Chains.UnitTests
{
    using System;
    using System.Security;
    using Chains.Play;
    using Chains.Play.Security.Provider;
    using Chains.Play.Web;
    using Chains.Play.Web.HttpListener;
    using Chains.UnitTests.Classes;
    using Chains.UnitTests.Classes.Security;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AuthorizationScenarioTest
    {
        private const string ValidSession = "1234";

        [TestMethod]
        public void IModuleRequirement_WhenSessionIsCorrectInASecuredContext_ThenChangesTheContext()
        {
            var executionChain = new ExecutionChain(new SecuredContextForTest());

            executionChain.Do(
                new ExecuteAction(
                    new SecuredAuthorizableActionForTest(
                        new ReproducibleTestData
                        {
                            ChangeToValue = "to-value"
                        })
                    {
                        Session = ValidSession
                    }));

            Assert.AreEqual("to-value", executionChain.CurrentContext.contextVariable);
        }

        [TestMethod]
        public void IModuleRequirement_WhenSessionIsWrongInASecuredContext_ThenThrowsException()
        {
            Exception exception = null;

            try
            {
                var executionChain = new ExecutionChain(new SecuredContextForTest());

                executionChain.Do(
                    new ExecuteAction(
                        new SecuredAuthorizableActionForTest(new ReproducibleTestData())
                        {
                            Session = "invalid"
                        }));
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            Assert.IsNotNull(exception);
            Assert.IsInstanceOfType(exception, typeof(SecurityException));
        }

        [TestMethod]
        public void IModuleRequirement_WhenTypeIsUnsecurableInASecuredContextThatAllowsUnsecuredActions_ThenExecutes()
        {
            var executionChain = new ExecutionChain(new SecuredContextForTest());

            executionChain.Do(
                new ExecuteAction(
                    new ActionForSecuredContextForTest(
                        new ReproducibleTestData
                        {
                            ChangeToValue = "to-value"
                        })));

            Assert.AreEqual("to-value", executionChain.CurrentContext.contextVariable);
        }

        [TestMethod]
        public void IModuleRequirement_WhenSessionIsWrongInASecuredExecutionChain_ThenThrowsException()
        {
            Exception exception = null;

            try
            {
                var executionChain = new ExecutionChain(new ContextForTest());
                executionChain.Modules.Add(new SecurityModuleForOnlyAuthorizedActions());

                executionChain.Do(new ExecuteAction(new ReproducibleTestAction(new ReproducibleTestData())));
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            Assert.IsNotNull(exception);
            Assert.IsInstanceOfType(exception, typeof(InvalidOperationException));
        }

        [TestMethod]
        public void IAuthorizationProvider_WhenProviderIsBeenSupported_ThenApplicationCanLoginAndUseSessionId()
        {
            const int port = 5769;

            // Test context with security
            var executionChain = new ExecutionChain(new SecuredContextForTest());
            executionChain.Modules.Add(new SecurityProvider(port));

            // The server that hosts the actual authentication url
            var server = new ServerHost(new Client("localhost", port));
            using (var httpServer = server.Do(new StartHttpServer()))
            {
                httpServer.Modules.Add(new HttpLoginProviderForTest(ValidSession, httpServer));

                // Get the authentication options from server
                var authorizationOptions =
                    executionChain.Do(
                        new ExecuteActionAsRemotable<ProviderAuthenticationGateData>(new GetAuthenticationGateOptions()));

                Assert.AreEqual(LoginUrlMethod.PostRequest, authorizationOptions.LoginUrlMethod);

                // After that login on the server that pointed from authorization server
                var responseResult = HttpRequest.DoRequest(
                    authorizationOptions.LoginUrl,
                    "post",
                    "username=" + HttpLoginProviderForTest.UserName + "&password=" + HttpLoginProviderForTest.Password,
                    HttpRequest.FormUrlEncodedContentType);

                Assert.IsFalse(responseResult.HasError);
                var resultedSession = responseResult.Response;

                Assert.AreEqual(ValidSession, resultedSession);

                // The use the session to make the request we need
                executionChain.Do(
                    new ExecuteAction(
                        new SecuredAuthorizableActionForTest(
                            new ReproducibleTestData
                            {
                                ChangeToValue = "to-value"
                            })
                        {
                            Session = resultedSession
                        }));
            }

            Assert.AreEqual("to-value", executionChain.CurrentContext.contextVariable);
        }
    }
}
