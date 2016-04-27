namespace Services.Communication.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Chains.Play;
    using Chains.Play.Web;
    using Chains.UnitTests.Classes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Services.Communication.Protocol;
    using Services.Communication.Tcp.Servers;
    using Services.Communication.UnitTests.Classes;
    using ProtocolType = System.Net.Sockets.ProtocolType;

    [TestClass]
    public class CommunicationBaseTest
    {
        private const string LocalhostIp = "127.0.0.1";

        [TestMethod]
        public void AsyncSocketListener_WhenStartedWithCorrectConfig_ThenServerIsUp()
        {
            var asyncSocketListener = new AsyncSocketListener(new ProtocolServerLogic(typeof(ContextForTest).FullName));

            try
            {
                asyncSocketListener.StartListening(new ServerHost(LocalhostIp, 5), 10);
            }
            finally
            {
                asyncSocketListener.StopListening();
            }
        }

        [TestMethod]
        public void AsyncSocketListener_WhenSocketObjectIsSent_ThenCallbackIsReceived()
        {
            var hasCalled = false;
            using (var server = new ServerHost(LocalhostIp, 7123).Do(
                        new StartListen(typeof(ContextForTest).FullName, onAfterExecute: x => hasCalled = true)))
            {
                SendMessage(7123);
                Thread.Sleep(1000);
                server.Close();
            }

            Assert.IsTrue(hasCalled);
        }

        [TestMethod]
        public void AsyncSocketListener_WhenSocketObjectIsSentInParallel_ThenCallbackIsReceived()
        {
            var hasCalled = false;
            using (var server = new ServerHost(LocalhostIp, 7125).Do(
                        new StartListen(typeof(ContextForTest).FullName, onAfterExecute: x => hasCalled = true)))
            {
                Parallel.For(0, 100, x =>
                {
                    SendMessage(7125);
                });
                
                Thread.Sleep(1000);
                server.Close();
            }

            Assert.IsTrue(hasCalled);
        }

        [TestMethod]
        public void AsyncSocketListener_WhenSocketObjectIsSentAndReceivedInParallel_ThenCallbackIsReceived()
        {
            var hasCalled = false;
            using (var server = new ServerHost(LocalhostIp, 7126).Do(
                        new StartListen(typeof(ContextForTest).FullName, onAfterExecute: x => hasCalled = true)))
            {
                Parallel.For(0, 100, x =>
                {
                    var item = (ReproducibleTestData)SendMessageAndReceive(7126).Data;
                    Assert.AreEqual("over tcp", item.ChangeToValue);
                });

                Thread.Sleep(1000);
                server.Close();
            }

            Assert.IsTrue(hasCalled);
        }

        [TestMethod]
        public void AsyncSocketListener_WhenMultipleServersAreSetup_ThenStoppingAndRestartingTheServerIsGuaranteed()
        {
            var testAction = new ReproducibleTestAction(
                new ReproducibleTestData
                {
                    ChangeToValue = "over tcp"
                });
            var hasCalled = false;

            using (var asyncSocketListener = new AsyncSocketListener(new ProtocolServerLogic(typeof(ContextForTest).FullName)))
            {
                asyncSocketListener.StartListening(new ServerHost(LocalhostIp, 7234), 5);

                using (var connection = new Client(LocalhostIp, 7234).Do(new OpenConnection()))
                {
                    connection.Do(testAction);
                }

                using (var connection = new Client(LocalhostIp, 7234).Do(new OpenConnection()))
                {
                    connection.Do(testAction);
                }

                using (var connection = new Client(LocalhostIp, 7234).Do(new OpenConnection()))
                {
                    connection.Do(testAction);
                }
            }

            using (var asyncSocketListener = new AsyncSocketListener(new ProtocolServerLogic(typeof(ContextForTest).FullName)))
            {
                asyncSocketListener.StartListening(new ServerHost(LocalhostIp, 7234), 5);
            }

            using (var asyncSocketListener = new AsyncSocketListener(new ProtocolServerLogic(typeof(ContextForTest).FullName)))
            {
                asyncSocketListener.StartListening(new ServerHost(LocalhostIp, 7234), 5);
            }

            using (var asyncSocketListener = new AsyncSocketListener(new ProtocolServerLogic(typeof(ContextForTest).FullName)))
            {
                asyncSocketListener.StartListening(new ServerHost(LocalhostIp, 7234), 5);
            }

            using (var asyncSocketListener = new AsyncSocketListener(new ProtocolServerLogic(typeof(ContextForTest).FullName)))
            {
                asyncSocketListener.StartListening(new ServerHost(LocalhostIp, 7234), 5);
            }

            var servertryFinally =
                new ServerHost(LocalhostIp, 7234).Do(
                    new StartListen(typeof(ContextForTest).FullName, onAfterExecute: x => hasCalled = true));
            try
            {
                using (var connection = new Client(LocalhostIp, 7234).Do(new OpenConnection()))
                {
                    connection.Do(testAction);
                }
            }
            finally
            {
                servertryFinally.Close();
            }

            servertryFinally =
                new ServerHost(LocalhostIp, 7234).Do(
                    new StartListen(typeof(ContextForTest).FullName, onAfterExecute: x => hasCalled = true));
            try
            {
                using (var connection = new Client(LocalhostIp, 7234).Do(new OpenConnection()))
                {
                    connection.Do(testAction);
                }
            }
            finally
            {
                servertryFinally.Close();
            }

            using (var server = new ServerHost(LocalhostIp, 7234).Do(
                            new StartListen(typeof(ContextForTest).FullName, onAfterExecute: x => hasCalled = true)))
            {
                using (var connection = new Client(LocalhostIp, 7234).Do(new OpenConnection()))
                {
                    connection.Do(testAction);
                }
                server.Close();
            }

            using (var server = new ServerHost(LocalhostIp, 7234).Do(
                            new StartListen(typeof(ContextForTest).FullName, onAfterExecute: x => hasCalled = true)))
            {
                using (var connection = new Client(LocalhostIp, 7234).Do(new OpenConnection()))
                {
                    connection.Do(testAction);
                }
                server.Close();
            }

            using (var server = new ServerHost(LocalhostIp, 7234).Do(
                            new StartListen(typeof(ContextForTest).FullName, onAfterExecute: x => hasCalled = true)))
            {
                using (var connection = new Client(LocalhostIp, 7234).Do(new OpenConnection()))
                {
                    connection.Do(testAction);
                }
                server.Close();
            }

            using (var server = new ServerHost(LocalhostIp, 7234).Do(
                            new StartListen(typeof(ContextForTest).FullName, onAfterExecute: x => hasCalled = true)))
            {
                using (var connection = new Client(LocalhostIp, 7234).Do(new OpenConnection()))
                {
                    connection.Do(testAction);
                }
                server.Close();
            }

            using (var server = new ServerHost(LocalhostIp, 7234).Do(
                            new StartListen(typeof(ContextForTest).FullName, onAfterExecute: x => hasCalled = true)))
            {
                using (var connection = new Client(LocalhostIp, 7234).Do(new OpenConnection()))
                {
                    connection.Do(testAction);
                }
                server.Close();
            }
        }

        [TestMethod]
        public void AsyncSocketListener_WhenSocketObjectIsExchanged_ThenHasTheSameText()
        {
            var hasCalled = false;
            using (new ServerHost(LocalhostIp, 7124).Do(
                        new StartListen(typeof(ContextForTest).FullName, onAfterExecute: x => hasCalled = true)))
            {
                var item = (ReproducibleTestData)SendMessageAndReceive(7124).Data;
                Thread.Sleep(1000);
                Assert.AreEqual("over tcp", item.ChangeToValue);
            }

            Assert.IsTrue(hasCalled);
        }

        [TestMethod]
        public void AsyncSocketListener_WhenSocketObjectIsExchangedWithUnsupportedCommand_ThenItReturnsUnknownRequest()
        {
            var hasCalled = false;
            using (new ServerHost(LocalhostIp, 7124).Do(
                        new StartListen(typeof(ContextForTest).FullName, onAfterExecute: x => hasCalled = true)))
            {
                var returnLine = SendUnsupportedMessageAndReceive();
                var deserializedMessage = DeserializableSpecification<ExecutableActionSpecification>.DeserializeFromJson(returnLine).Data;
                Assert.IsInstanceOfType(deserializedMessage, typeof(InvalidOperationException));
            }

            Assert.IsFalse(hasCalled);
        }

        [TestMethod]
        public void AsyncSocketListener_WhenClientSends_ThenHasTheSameText()
        {
            var testAction = new ReproducibleTestAction(
                new ReproducibleTestData
                {
                    ChangeToValue = "over tcp"
                });

            var hasCalled = false;
            using (new ServerHost(LocalhostIp, 3997).Do(
                        new StartListen(typeof(ContextForTest).FullName, onAfterExecute: x => hasCalled = true)))
            {
                using (var connection = new Client(LocalhostIp, 3997).Do(new OpenConnection()))
                {
                    connection.Do(testAction);
                }
            }

            Assert.IsTrue(hasCalled);
        }

        [TestMethod]
        public void AsyncSocketListener_WhenClientSendsUsingHttp_ThenHasTheSameText()
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
                    var connection =
                        new Client("localhost", 3996, "/custom path/awesome service").Do(
                            new OpenConnection(protocolType: Protocol.ProtocolType.Http)))
                {
                    connection.Do(testAction);
                }
            }

            Assert.IsTrue(hasCalled);
        }

        [TestMethod]
        public void AsyncSocketListener_WhenClientSendsButNoReceiveUsingHttp_ThenHasNoContent()
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
                                                 .Do(new SendNoReply(testAction)))
                {
                }
            }

            Assert.IsTrue(hasCalled);
        }

        [TestMethod]
        public void AsyncSocketListener_WhenClientSendsUsingHttp_ThenMultipleServersCanBeHosted()
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
                        var connection =
                            new Client("localhost", 3995, "/custom-path").Do(
                                new OpenConnection(protocolType: Protocol.ProtocolType.Http)))
                    {
                        connection.Do(testAction);
                    }

                    using (
                        var connection =
                            new Client("localhost", 3995, "/custom-path-2").Do(
                                new OpenConnection(protocolType: Protocol.ProtocolType.Http)))
                    {
                        connection.Do(testAction);
                    }
                }
            }

            Assert.IsTrue(hasCalled);
        }

        [TestMethod]
        public void AsyncSocketListener_WhenClientSendsOneThousandItems_ThenHasTheSameText()
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
            using (new ServerHost(LocalhostIp, 15010).Do(
                        new StartListen(typeof(ContextForTest).FullName, onAfterExecute: x => hasCalled = true)))
            {
                using (new Client(LocalhostIp, 15010).Do(new OpenConnection()).Do(items))
                {
                }
            }

            Assert.IsTrue(hasCalled);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void AsyncSocketListener_WhenClientSendsNotSupportedAction_ThenItDoesNotExecute()
        {
            var testAction = new ReproducibleTestAction2(
                new ReproducibleTestData
                {
                    ChangeToValue = "over tcp"
                });

            using (new ServerHost(LocalhostIp, 15002).Do(new StartListen(typeof(ContextForTest).FullName)))
            {
                using (new Client(LocalhostIp, 15002).Do(new OpenConnection()).Do(testAction))
                {
                }
            }
        }

        [TestMethod]
        public void AsyncSocketListener_WhenServerCreatesAnError_ThenTheErrorIsCorrectlyPropagated()
        {
            var testAction = new ReproducibleTestActionWithError(
                new ReproducibleTestData
                {
                    ChangeToValue = "over tcp"
                });

            try
            {
                using (new ServerHost(LocalhostIp, 15002).Do(new StartListen(typeof(ContextForTest2).FullName)))
                {
                    using (new Client(LocalhostIp, 15002).Do(new OpenConnection()).Do(testAction))
                    {
                    }
                }
            }
            catch (KeyNotFoundException expectedException)
            {
                Assert.AreEqual(ReproducibleTestActionWithError.ExceptionText, expectedException.Message);
                Assert.AreEqual(ReproducibleTestActionWithError.InnerExceptionText, expectedException.InnerException.Message);
            }
        }

        private void SendMessage(int port)
        {
            var testAction = new ReproducibleTestAction(
                new ReproducibleTestData
                {
                    ChangeToValue = "over tcp"
                });

            var commandText =
                $"TcpCommand {SerializableSpecification.SerializeManyToJson(new[] { testAction.GetInstanceSpec() })}{Environment.NewLine}";

            using (var socket = CreateClient(port))
            {
                using (var socketStream = new NetworkStream(socket))
                {
                    using (var writer = new StreamWriter(socketStream, Encoding.ASCII))
                    {
                        writer.Write(commandText);
                        writer.Flush();
                    }
                }

                socket.Close();
                socket.Dispose();
            }
        }

        private ExecutableActionSpecification SendMessageAndReceive(int port)
        {
            var testAction = new ReproducibleTestAction(
                new ReproducibleTestData
                {
                    ChangeToValue = "over tcp"
                });

            using (var socket = CreateClient(port))
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
                        writer.Write("{0} ", command);
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
            var serverAddress = new IPEndPoint(IPAddress.Parse(LocalhostIp), port);
            var socket = new Socket(serverAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            socket.Connect(serverAddress);

            return socket;
        }
    }
}
