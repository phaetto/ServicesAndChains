namespace Services.Communication.UnitTests
{
    using System;
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
    using ProtocolType = System.Net.Sockets.ProtocolType;

    [TestClass]
    public class CommunicationV2BaseTest
    {
        private const string LocalhostIp = "127.0.0.1";

        [TestMethod]
        public void AsyncSocketListener_WhenSocketObjectIsSent_ThenCallbackIsReceived()
        {
            var hasCalled = false;
            using (var server = new ServerHost(LocalhostIp, 7153).Do(
                        new StartListen(typeof(ContextForTest).FullName, onAfterExecute: x => hasCalled = true)))
            {
                SendMessage(7153);
                Thread.Sleep(1000);
                server.Close();
            }

            Assert.IsTrue(hasCalled);
        }

        [TestMethod]
        public void AsyncSocketListener_WhenV2SupportIsRequested_ThenCallbackIsReceived()
        {
            using (new ServerHost(LocalhostIp, 7154).Do(
                new StartListen(typeof (ContextForTest).FullName)))
            {
                var response = QueryV2ProtocolVersion(7154);
                Assert.AreEqual("ok", response);
            }
        }

        [TestMethod]
        public void AsyncSocketListener_WhenSocketObjectIsSentAndReceivedInParallel_ThenCallbackIsReceived()
        {
            var hasCalled = false;
            using (var server = new ServerHost(LocalhostIp, 7156).Do(
                        new StartListen(typeof(ContextForTest).FullName, onAfterExecute: x => hasCalled = true)))
            {
                Parallel.For(0, 100, x =>
                {
                    var item = (ReproducibleTestData)SendMessageAndReceive(7156).Data;
                    Assert.AreEqual("over tcp", item.ChangeToValue);
                });

                Thread.Sleep(1000);
                server.Close();
            }

            Assert.IsTrue(hasCalled);
        }

        private void SendMessage(int port)
        {
            var testAction = new ReproducibleTestAction(
                new ReproducibleTestData
                {
                    ChangeToValue = "over tcp"
                });

            var commandText = StateObject.V2Header.PadRight(StateObject.HeaderDefaultBufferSize);
            var valueToSend = SerializableSpecification.SerializeManyToJson(new[] { testAction.GetInstanceSpec() });

            using (var socket = CreateClient(port))
            {
                using (var socketStream = new NetworkStream(socket))
                {
                    using (var writer = new StreamWriter(socketStream, Encoding.ASCII, commandText.Length, true))
                    {
                        // Header and features
                        writer.Write(commandText);
                        writer.Flush();
                    }

                    using (var writer = new BinaryWriter(socketStream, Encoding.ASCII))
                    {
                        // Message size
                        long size = Encoding.UTF8.GetByteCount(valueToSend);
                        var sizeToBytes = BitConverter.GetBytes(size);
                        Assert.IsTrue(sizeToBytes.Length == 8);
                        writer.Write(sizeToBytes);
                        writer.Flush();

                        // Binary message size (after encryption/gzip)
                        writer.Write(sizeToBytes);
                        writer.Flush();

                        // Message (in binary)
                        var bytes = new byte[size];
                        Encoding.UTF8.GetBytes(valueToSend, 0, valueToSend.Length, bytes, 0);
                        writer.Write(bytes);

                        // Terminating sequence todo: do we need that?
                    }
                }

                socket.Close();
                socket.Dispose();
            }
        }

        private string QueryV2ProtocolVersion(int port)
        {
            var commandText = StateObject.V2SupportRequestHeader.PadRight(StateObject.HeaderDefaultBufferSize);

            using (var socket = CreateClient(port))
            {
                using (var socketStream = new NetworkStream(socket))
                {
                    using (var writer = new StreamWriter(socketStream, Encoding.ASCII, commandText.Length, true))
                    {
                        // Header and features
                        writer.Write(commandText);
                        writer.Flush();
                    }

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
                        writer.Write(
                            SerializableSpecification.SerializeManyToJson(
                                new[]
                                {
                                    testAction.GetInstanceSpec()
                                }));

                        writer.Write("\r\n");
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

        private Socket CreateClient(int port)
        {
            var serverAddress = new IPEndPoint(IPAddress.Parse(LocalhostIp), port);
            var socket = new Socket(serverAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            socket.Connect(serverAddress);

            return socket;
        }
    }
}
