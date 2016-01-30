namespace Chains.UnitTests
{
    using System.Threading.Tasks;
    using Chains.Play.Web;
    using Chains.Play.Web.HttpListener;
    using Chains.UnitTests.Classes.Http;
    using Chains.UnitTests.Classes.Security;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class HttpListenerTest
    {
        [TestMethod]
        public void HttpServer_WhenHttpServerIsCalledAsAModule_ThenItSendsTheCorrectText()
        {
            var server = new ServerHost(new Client("localhost", 9100));

            using (var httpServer = server.Do(
                new StartHttpServer(
                    new[]
                    {
                        "/test/"
                    })))
            {
                httpServer.Modules.Add(new HttpContextForTest("output text"));
                var responseResult = HttpRequest.DoRequest("http://localhost:9100/test/");

                Assert.IsFalse(responseResult.HasError);
                Assert.AreEqual("output text", responseResult.Response);
            }
        }

        [TestMethod]
        public void HttpServer_WhenHttpServerDoesNotServeAnUri_Then404ShouldBeSentAsResponse()
        {
            var server = new ServerHost(new Client("localhost", 9100));

            using (var httpServer = server.Do(
                new StartHttpServer(
                    new[]
                    {
                        "/test/"
                    })))
            {
                httpServer.Modules.Add(new HttpContextForTest("output text"));
                var responseResult = HttpRequest.DoRequest("http://localhost:9100/does-not-exists/");

                Assert.IsTrue(responseResult.HasError);
                Assert.IsTrue(responseResult.Response.Contains("Not Found"));
            }
        }

        [TestMethod]
        public void HttpServer_WhenHttpServerDoesNotSupportAnUri_Then404ShouldBeSentAsResponse()
        {
            var server = new ServerHost(new Client("localhost", 9100));

            using (var httpServer = server.Do(
                new StartHttpServer(
                    new[]
                    {
                        "/test/"
                    })))
            {
                httpServer.Modules.Add(new HttpContextForTestNotSupportAnything("output text"));
                var responseResult = HttpRequest.DoRequest("http://localhost:9100/test/");

                Assert.IsTrue(responseResult.HasError);
                Assert.IsTrue(responseResult.Response.Contains("Not Found"));
            }
        }

        [TestMethod]
        public void HttpServer_WhenHttpServerIsCalledAsAModuleAndErrorHappens_ThenItSends500()
        {
            var server = new ServerHost(new Client("localhost", 9100));

            using (var httpServer = server.Do(
                new StartHttpServer(
                    new[]
                    {
                        "/test-error/"
                    })))
            {
                httpServer.Modules.Add(new HttpContextForTestWithError("output text"));
                var responseResult = HttpRequest.DoRequest("http://localhost:9100/test-error/");

                Assert.IsTrue(responseResult.HasError);
                Assert.AreNotEqual("output text", responseResult.Response);
            }
        }
    }
}
