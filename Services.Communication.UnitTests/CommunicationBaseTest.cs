namespace Services.Communication.UnitTests
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using Chains.Play;
    using Chains.Play.Web;
    using Chains.UnitTests.Classes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Services.Communication.Protocol;
    using Services.Communication.Tcp.Servers;
    using SuperSocket.SocketBase.Config;
    using SuperSocket.SocketEngine;
    using ProtocolType = System.Net.Sockets.ProtocolType;

    [TestClass]
    public class CommunicationBaseTest
    {
        [TestMethod]
        public void TcpServer_WhenStartedWithCorrectConfig_ThenServerIsUp()
        {
            var appServer = new TcpServer(new ProtocolServerLogic(typeof(ContextForTest).FullName));

            try
            {
                var serverConfig = new ServerConfig();
                serverConfig.Ip = "127.0.0.1";
                serverConfig.Port = 4000;
                serverConfig.Name = "myserver";

                var setupResult = appServer.Setup(new RootConfig(), serverConfig, new SocketServerFactory());

                Assert.IsTrue(setupResult);
                Assert.IsTrue(appServer.Start());
            }
            finally
            {
                appServer.Stop();
            }
        }

        [TestMethod]
        public void TcpServer_WhenSocketObjectIsSent_ThenCallbackIsReceived()
        {
            var hasCalled = false;
            using (var server = new ServerHost("127.0.0.1", 7123).Do(
                        new StartListen(typeof(ContextForTest).FullName, onAfterExecute: x => hasCalled = true)))
            {
                SendMessage();
                Thread.Sleep(1000);
                server.Close();
            }

            Assert.IsTrue(hasCalled);
        }

        [TestMethod]
        public void TcpServer_WhenMultipleServersAreSetup_ThenStoppingAndRestartingTheServerIsGuaranteed()
        {
            var testAction = new ReproducibleTestAction(
                new ReproducibleTestData
                {
                    ChangeToValue = "over tcp"
                });
            var hasCalled = false;

            using (var appServer = new TcpServer(new ProtocolServerLogic(typeof(ContextForTest).FullName)))
            {
                var serverConfig = new ServerConfig();
                serverConfig.Ip = "127.0.0.1";
                serverConfig.Port = 7234;
                serverConfig.Name = "test0F3";
                serverConfig.KeepAliveTime = 3;
                serverConfig.MaxConnectionNumber = 10000;

                var setupResult = appServer.Setup(new RootConfig(), serverConfig, new SocketServerFactory());

                Assert.IsTrue(setupResult);
                Assert.IsTrue(appServer.Start());

                using (new Client("127.0.0.1", 7234).Do(new OpenConnection()).Do(new Send(testAction)))
                {
                }

                using (new Client("127.0.0.1", 7234).Do(new OpenConnection()).Do(new Send(testAction)))
                {
                }

                using (new Client("127.0.0.1", 7234).Do(new OpenConnection()).Do(new Send(testAction)))
                {
                }
            }

            using (var appServer = new TcpServer(new ProtocolServerLogic(typeof(ContextForTest).FullName)))
            {
                var serverConfig = new ServerConfig();
                serverConfig.Ip = "127.0.0.1";
                serverConfig.Port = 7234;
                serverConfig.Name = "test0F3";

                var setupResult = appServer.Setup(new RootConfig(), serverConfig, new SocketServerFactory());

                Assert.IsTrue(setupResult);
                Assert.IsTrue(appServer.Start());
            }

            using (var appServer = new TcpServer(new ProtocolServerLogic(typeof(ContextForTest).FullName)))
            {
                var serverConfig = new ServerConfig();
                serverConfig.Ip = "127.0.0.1";
                serverConfig.Port = 7234;
                serverConfig.Name = "test0F3";

                var setupResult = appServer.Setup(new RootConfig(), serverConfig, new SocketServerFactory());

                Assert.IsTrue(setupResult);
                Assert.IsTrue(appServer.Start());
            }

            using (var appServer = new TcpServer(new ProtocolServerLogic(typeof(ContextForTest).FullName)))
            {
                var serverConfig = new ServerConfig();
                serverConfig.Ip = "127.0.0.1";
                serverConfig.Port = 7234;
                serverConfig.Name = "test0F3";

                var setupResult = appServer.Setup(new RootConfig(), serverConfig, new SocketServerFactory());

                Assert.IsTrue(setupResult);
                Assert.IsTrue(appServer.Start());
            }

            using (var appServer = new TcpServer(new ProtocolServerLogic(typeof(ContextForTest).FullName)))
            {
                var serverConfig = new ServerConfig();
                serverConfig.Ip = "127.0.0.1";
                serverConfig.Port = 7234;
                serverConfig.Name = "test0F3";

                var setupResult = appServer.Setup(new RootConfig(), serverConfig, new SocketServerFactory());

                Assert.IsTrue(setupResult);
                Assert.IsTrue(appServer.Start());
            }

            var servertryFinally =
                new ServerHost("127.0.0.1", 7234).Do(
                    new StartListen(typeof(ContextForTest).FullName, onAfterExecute: x => hasCalled = true));
            try
            {
                using (new Client("127.0.0.1", 7234).Do(new OpenConnection()).Do(new Send(testAction)))
                {
                }
            }
            finally
            {
                servertryFinally.Close();
            }

            servertryFinally =
                new ServerHost("127.0.0.1", 7234).Do(
                    new StartListen(typeof(ContextForTest).FullName, onAfterExecute: x => hasCalled = true));
            try
            {
                using (new Client("127.0.0.1", 7234).Do(new OpenConnection()).Do(new Send(testAction)))
                {
                }
            }
            finally
            {
                servertryFinally.Close();
            }

            using (var server = new ServerHost("127.0.0.1", 7234).Do(
                            new StartListen(typeof(ContextForTest).FullName, onAfterExecute: x => hasCalled = true)))
            {
                using (new Client("127.0.0.1", 7234).Do(new OpenConnection()).Do(new Send(testAction)))
                {
                }
                server.Close();
            }

            using (var server = new ServerHost("127.0.0.1", 7234).Do(
                            new StartListen(typeof(ContextForTest).FullName, onAfterExecute: x => hasCalled = true)))
            {
                using (new Client("127.0.0.1", 7234).Do(new OpenConnection()).Do(new Send(testAction)))
                {
                }
                server.Close();
            }

            using (var server = new ServerHost("127.0.0.1", 7234).Do(
                            new StartListen(typeof(ContextForTest).FullName, onAfterExecute: x => hasCalled = true)))
            {
                using (new Client("127.0.0.1", 7234).Do(new OpenConnection()).Do(new Send(testAction)))
                {
                }
                server.Close();
            }

            using (var server = new ServerHost("127.0.0.1", 7234).Do(
                            new StartListen(typeof(ContextForTest).FullName, onAfterExecute: x => hasCalled = true)))
            {
                using (new Client("127.0.0.1", 7234).Do(new OpenConnection()).Do(new Send(testAction)))
                {
                }
                server.Close();
            }

            using (var server = new ServerHost("127.0.0.1", 7234).Do(
                            new StartListen(typeof(ContextForTest).FullName, onAfterExecute: x => hasCalled = true)))
            {
                using (new Client("127.0.0.1", 7234).Do(new OpenConnection()).Do(new Send(testAction)))
                {
                }
                server.Close();
            }
        }

        [TestMethod]
        public void TcpServer_WhenSocketObjectIsExchanged_ThenHasTheSameText()
        {
            var hasCalled = false;
            using (new ServerHost("127.0.0.1", 7124).Do(
                        new StartListen(typeof(ContextForTest).FullName, onAfterExecute: x => hasCalled = true)))
            {
                var item = (ReproducibleTestData)SendMessageAndReceive().Data;
                Thread.Sleep(1000);
                Assert.AreEqual("over tcp", item.ChangeToValue);
            }

            Assert.IsTrue(hasCalled);
        }

        [TestMethod]
        public void TcpServer_WhenSocketObjectIsExchangedWithUnsupportedCommand_ThenItReturnsUnknownRequest()
        {
            var hasCalled = false;
            using (new ServerHost("127.0.0.1", 7124).Do(
                        new StartListen(typeof(ContextForTest).FullName, onAfterExecute: x => hasCalled = true)))
            {
                var returnLine = SendUnsupportedMessageAndReceive();
                Assert.AreEqual("Unknown request: WrongTcpCommand", returnLine);
            }

            Assert.IsFalse(hasCalled);
        }

        [TestMethod]
        public void TcpServer_WhenClientSends_ThenHasTheSameText()
        {
            var testAction = new ReproducibleTestAction(
                new ReproducibleTestData
                {
                    ChangeToValue = "over tcp"
                });

            var hasCalled = false;
            using (new ServerHost("127.0.0.1", 3997).Do(
                        new StartListen(typeof(ContextForTest).FullName, onAfterExecute: x => hasCalled = true)))
            {
                using (new Client("127.0.0.1", 3997).Do(new OpenConnection()).Do(new Send(testAction)))
                {
                }
            }

            Assert.IsTrue(hasCalled);
        }

        [TestMethod]
        public void TcpServer_WhenClientSendsUsingHttp_ThenHasTheSameText()
        {
            var testAction = new ReproducibleTestAction(
                new ReproducibleTestData
                {
                    ChangeToValue = "over http"
                });

            var hasCalled = false;
            using (
                new ServerHost("localhost", 3996, "/custom path/awesome service").Do(
                    new StartListen(typeof(ContextForTest).FullName,
                        onAfterExecute: x => hasCalled = true,
                        protocolType: Protocol.ProtocolType.Http)))
            {
                using (
                    new Client("localhost", 3996, "/custom path/awesome service").Do(new OpenConnection(protocolType: Protocol.ProtocolType.Http))
                                                 .Do(new Send(testAction)))
                {
                }
            }

            Assert.IsTrue(hasCalled);
        }

        [TestMethod]
        public void TcpServer_WhenClientSendsButNoReceiveUsingHttp_ThenHasNoContent()
        {
            var testAction = new ReproducibleTestAction(
                new ReproducibleTestData
                {
                    ChangeToValue = "over http"
                });

            var hasCalled = false;
            using (
                new ServerHost("localhost", 3996, "/custom path/awesome service").Do(
                    new StartListen(typeof(ContextForTest).FullName,
                        onAfterExecute: x => hasCalled = true,
                        protocolType: Protocol.ProtocolType.Http)))
            {
                using (
                    new Client("localhost", 3996, "/custom path/awesome service").Do(new OpenConnection(protocolType: Protocol.ProtocolType.Http))
                                                 .Do(new Send(testAction, expectReply: false)))
                {
                }
            }

            Assert.IsTrue(hasCalled);
        }

        [TestMethod]
        public void TcpServer_WhenClientSendsUsingHttp_ThenMultipleServersCanBeHosted()
        {
            var testAction = new ReproducibleTestAction(
                new ReproducibleTestData
                {
                    ChangeToValue = "over http"
                });

            var hasCalled = false;
            using (
                new ServerHost("localhost", 3995, "/custom-path").Do(
                    new StartListen(typeof(ContextForTest).FullName,
                        onAfterExecute: x => hasCalled = true,
                        protocolType: Protocol.ProtocolType.Http)))
            {
                using (
                    new ServerHost("localhost", 3995, "/custom-path-2").Do(
                        new StartListen(typeof(ContextForTest).FullName,
                            protocolType: Protocol.ProtocolType.Http)))
                {
                    using (
                        new Client("localhost", 3995, "/custom-path").Do(
                            new OpenConnection(protocolType: Protocol.ProtocolType.Http))
                                                                     .Do(new Send(testAction)))
                    {
                    }

                    using (
                        new Client("localhost", 3995, "/custom-path-2").Do(
                            new OpenConnection(protocolType: Protocol.ProtocolType.Http))
                                                                     .Do(new Send(testAction)))
                    {
                    }
                }
            }

            Assert.IsTrue(hasCalled);
        }

        [TestMethod]
        public void TcpServer_WhenClientSendsOneThousandItems_ThenHasTheSameText()
        {
            var testAction = new ReproducibleTestAction(
                new ReproducibleTestData
                {
                    ChangeToValue = "over tcp"
                });

            var items = new ExecutableActionSpecification[1000];
            for (int i = 0; i < items.Length; ++i)
            {
                items[i] = testAction.GetInstanceSpec();
            }

            var hasCalled = false;
            using (new ServerHost("127.0.0.1", 15010).Do(
                        new StartListen(typeof(ContextForTest).FullName, onAfterExecute: x => hasCalled = true)))
            {
                using (new Client("127.0.0.1", 15010).Do(new OpenConnection()).Do(new Send(items)))
                {
                }
            }

            Assert.IsTrue(hasCalled);
        }

        [TestMethod]
        public void TcpServer_WhenClientSendsNotSupportedAction_ThenItDoesNotExecute()
        {
            var testAction = new ReproducibleTestAction2(
                new ReproducibleTestData
                {
                    ChangeToValue = "over tcp"
                });

            var hasCalled = false;
            Exception exception = null;

            try
            {
                using (new ServerHost("127.0.0.1", 15002).Do(new StartListen(typeof(ContextForTest).FullName)))
                {
                    using (
                        new Client("127.0.0.1", 15002).Do(new OpenConnection())
                                                      .Do(new Send(testAction)))
                    {
                        Assert.Fail("Execution should never reach here");
                    }
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            Assert.IsNotNull(exception);
        }

        private void SendMessage()
        {
            var testAction = new ReproducibleTestAction(
                new ReproducibleTestData
                {
                    ChangeToValue = "over tcp"
                });

            using (var socket = CreateClient(7123))
            {
                using (var socketStream = new NetworkStream(socket))
                {
                    using (var writer = new StreamWriter(socketStream, Encoding.ASCII))
                    {
                        writer.Write(string.Format("TcpCommand "));
                        writer.Write(
                            SerializableSpecification.SerializeManyToJson(
                                new[]
                                {
                                    testAction.GetInstanceSpec()
                                }));

                        writer.Write(string.Format("\r\n"));
                        writer.Flush();
                    }
                }

                socket.Close();
                socket.Dispose();
            }
        }

        private ExecutableActionSpecification SendMessageAndReceive()
        {
            var testAction = new ReproducibleTestAction(
                new ReproducibleTestData
                {
                    ChangeToValue = "over tcp"
                });

            using (var socket = CreateClient(7124))
            {
                using (var socketStream = new NetworkStream(socket))
                {
                    using (var writer = new StreamWriter(socketStream, Encoding.ASCII))
                    {
                        writer.Write("TcpCommand ");
                        writer.Write(
                            SerializableSpecification.SerializeManyToJson(
                                new[]
                                {
                                    testAction.GetInstanceSpec()
                                }));

                        writer.Write(string.Format("\r\n"));
                        writer.Flush();

                        using (var reader = new StreamReader(socketStream, Encoding.ASCII))
                        {
                            var line = reader.ReadLine();
                            socket.Close();
                            socket.Dispose();
                            return DeserializableSpecification<ExecutableActionSpecification>.DeserializeFromJson(line);
                        }
                    }
                }
            }
        }

        private string SendUnsupportedMessageAndReceive(string command = "WrongTcpCommand")
        {
            var testAction = new ReproducibleTestAction(
                new ReproducibleTestData
                {
                    ChangeToValue = "over tcp"
                });

            using (var socket = CreateClient(7124))
            {
                using (var socketStream = new NetworkStream(socket))
                {
                    using (var writer = new StreamWriter(socketStream, Encoding.ASCII))
                    {
                        writer.Write(string.Format("{0} ", command));
                        writer.Write(
                            SerializableSpecification.SerializeManyToJson(
                                new[]
                                {
                                    testAction.GetInstanceSpec()
                                }));

                        writer.Write(string.Format("\r\n"));
                        writer.Flush();

                        using (var reader = new StreamReader(socketStream, Encoding.ASCII))
                        {
                            var line = reader.ReadLine();
                            socket.Close();
                            socket.Dispose();
                            return line;
                        }
                    }
                }
            }
        }

        private Socket CreateClient(int port)
        {
            var serverAddress = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            var socket = new Socket(serverAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            socket.Connect(serverAddress);

            return socket;
        }
    }
}
