namespace Services.Communication.Tcp.Servers
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using Chains.Exceptions;
    using Chains.Play.Web;
    using Services.Communication.Protocol;
    using ProtocolType = System.Net.Sockets.ProtocolType;

    public class AsyncSocketListener : IDisposable
    {
        private const int DisposeWaitTimeoutInMilliseconds = 1000;
        private const string CommandSeparator = "\r\n";
        private const char CommandSeparatorChar = '\n';

        private readonly ManualResetEvent manualResetEvent = new ManualResetEvent(false);

        internal readonly ProtocolServerLogic ProtocolServerLogic;

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private readonly Thread connectionThread;

        private readonly ManualResetEvent waitToStopManualResetEvent = new ManualResetEvent(false);
        
        private int pendingConnectionsLimit;

        private IPEndPoint ipEndpoint;

        public AsyncSocketListener(ProtocolServerLogic protocolServerLogic)
        {
            ProtocolServerLogic = protocolServerLogic;
            connectionThread = new Thread(ConnectionThread_Start);
        }

        private void ConnectionThread_Start()
        {
            var cancellationToken = cancellationTokenSource.Token;

            using (var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
                listener.NoDelay = true;

                listener.Bind(ipEndpoint);
                listener.Listen(pendingConnectionsLimit);

                while (!cancellationToken.IsCancellationRequested)
                {
                    manualResetEvent.Reset();
                    listener.BeginAccept(OnClientConnect, listener);
                    WaitHandle.WaitAny(new[] { cancellationToken.WaitHandle, manualResetEvent });
                }
            }

            waitToStopManualResetEvent.Set();
        }

        public void StartListening(ServerHost context, int pendingConnectionsLimit)
        {

            ipEndpoint = new IPEndPoint(IPAddress.Parse(context.Parent.Hostname), context.Parent.Port);
            this.pendingConnectionsLimit = pendingConnectionsLimit;

            connectionThread.Start();
        }

        public void StopListening()
        {
            cancellationTokenSource.Cancel();
            waitToStopManualResetEvent.WaitOne();
        }

        private void OnClientConnect(IAsyncResult result)
        {
            manualResetEvent.Set();
            var state = new StateObject();

            try
            {
                var socket = (Socket)result.AsyncState;
                state.Listener = socket.EndAccept(result);
                state.Listener.BeginReceive(state.Buffer, 0, StateObject.HeaderDefaultBufferSize, SocketFlags.None, ReceiveMessageHeaderCallback, state);
            }
            catch (SocketException)
            {
                Close(state);
            }
            catch (ObjectDisposedException)
            {
                Close(state);
            }
        }

        public void ReceiveMessageHeaderCallback(IAsyncResult result)
        {
            var state = (StateObject)result.AsyncState;

            try
            {
                var receivedBytes = state.Listener.EndReceive(result);

                var headerString = Encoding.ASCII.GetString(state.Buffer, 0, receivedBytes);

                if (headerString.StartsWith(StateObject.V1Header))
                {
                    // Text format, we do not know the size, no encryption, no gzip
                    state.ProtocolVersionSelected = 1;
                    state.StringBuilder.Append(headerString);
                    state.Buffer = new byte[state.BufferSize];
                    state.Listener.BeginReceive(state.Buffer, 0, state.BufferSize, SocketFlags.None, ReceiveCallbackV1, state);
                }
                else if (headerString.StartsWith(StateObject.V2Header))
                {
                    state.ProtocolVersionSelected = 2;

                    // Extract features
                    headerString = headerString.Substring(StateObject.V2Header.Length);

                    // Next 8 bytes is size
                    state.BufferSize = 8;
                    state.Buffer = new byte[state.BufferSize];
                    state.Listener.BeginReceive(state.Buffer, 0, state.BufferSize, SocketFlags.None, ReceiveV2MessageSize, state);
                }
                else if (headerString.StartsWith(StateObject.V2SupportRequestHeader))
                {
                    state.ProtocolVersionSelected = 2;
                    Send(state, "ok");
                }
                else
                {
                    Send(state, ProtocolServerLogic.Serialize(new InvalidOperationException("Header is not supported")));
                }
            }
            catch (SocketException)
            {
                Close(state);
            }
        }

        public void ReceiveV2MessageSize(IAsyncResult result)
        {
            var state = (StateObject)result.AsyncState;

            try
            {
                var receivedBytes = state.Listener.EndReceive(result);

                if (receivedBytes != 8)
                {
                    Send(state, ProtocolServerLogic.Serialize(new InvalidOperationException("V2 protocol error: 8 bytes expected with message size")));
                    return;
                }

                state.ActualMessageSize = BitConverter.ToInt64(state.Buffer, state.BufferSize);

                // Next 8 bytes is binary size
                state.BufferSize = 8;
                state.Buffer = new byte[state.BufferSize];
                state.Listener.BeginReceive(state.Buffer, 0, state.BufferSize, SocketFlags.None, ReceiveV2BinaryMessageSize, state);
            }
            catch (SocketException)
            {
                Close(state);
            }
        }

        public void ReceiveV2BinaryMessageSize(IAsyncResult result)
        {
            var state = (StateObject)result.AsyncState;

            try
            {
                var receivedBytes = state.Listener.EndReceive(result);

                if (receivedBytes != 8)
                {
                    Send(state, ProtocolServerLogic.Serialize(new InvalidOperationException("V2 protocol error: 8 bytes expected with binary message size")));
                    return;
                }

                state.BinaryMessageSize = BitConverter.ToInt64(state.Buffer, state.BufferSize);

                // Next bytes is the actual message
                state.BufferSize = StateObject.BodyDefaultBufferSize;
                state.Buffer = new byte[state.BufferSize];
                state.Listener.BeginReceive(state.Buffer, 0, state.BufferSize, SocketFlags.None, ReceiveCallbackV2, state);
            }
            catch (SocketException)
            {
                Close(state);
            }
        }

        public void ReceiveCallbackV1(IAsyncResult result)
        {
            var state = (StateObject)result.AsyncState;

            try
            {
                var receivedBytes = state.Listener.EndReceive(result);
                var receivedString = Encoding.ASCII.GetString(state.Buffer, 0, receivedBytes);
                state.StringBuilder.Append(receivedString);

                if (receivedString.Contains(CommandSeparator))
                {
                    var commandTextArray = state.StringBuilder.ToString().Split(CommandSeparatorChar);

                    var commandText = commandTextArray[0];
                    commandText = commandText.Substring(commandText.IndexOf('['));

                    if (string.IsNullOrWhiteSpace(commandText))
                    {
                        Send(state, ProtocolServerLogic.Serialize(new InvalidOperationException("The command body was empty.")));
                    }
                    else
                    {
                        var response = ProtocolServerLogic.ReadFromStreamAndPlay(commandText);

                        if (!string.IsNullOrWhiteSpace(response))
                        {
                            Send(state, response);
                        }
                        else
                        {
                            Send(state, ProtocolServerLogic.Serialize(new InvalidOperationException("The server could not return an answer.")));
                        }
                    }

                    if (commandTextArray.Length > 1 && !string.IsNullOrEmpty(commandTextArray[commandTextArray.Length - 1]))
                    {
                        state.StringBuilder = new StringBuilder(commandTextArray[commandTextArray.Length - 1]);
                    }
                    else
                    {
                        state.StringBuilder = new StringBuilder();
                    }

                    state.Listener.BeginReceive(state.Buffer, 0, state.BufferSize, SocketFlags.None, ReceiveCallbackV1, state);
                }
                else
                {
                    if (receivedBytes > 0)
                    {
                        state.Listener.BeginReceive(state.Buffer, 0, state.BufferSize, SocketFlags.None,
                            ReceiveCallbackV1, state);
                    }
                }
            }
            catch (NullReferenceException)
            {
                Close(state);
            }
            catch (SocketException)
            {
                Close(state);
            }
        }

        public void ReceiveCallbackV2(IAsyncResult result)
        {
            var state = (StateObject)result.AsyncState;

            try
            {
                var receivedBytes = state.Listener.EndReceive(result);
                var receivedString = Encoding.ASCII.GetString(state.Buffer, 0, receivedBytes);
                state.StringBuilder.Append(receivedString);

                if (receivedString.Contains(CommandSeparator))
                {
                    var commandTextArray = state.StringBuilder.ToString().Split(CommandSeparatorChar);

                    var commandText = commandTextArray[0];
                    commandText = commandText.Substring(commandText.IndexOf('['));

                    if (string.IsNullOrWhiteSpace(commandText))
                    {
                        Send(state, ProtocolServerLogic.Serialize(new InvalidOperationException("The command body was empty.")));
                    }
                    else
                    {
                        var response = ProtocolServerLogic.ReadFromStreamAndPlay(commandText);

                        if (!string.IsNullOrWhiteSpace(response))
                        {
                            Send(state, response);
                        }
                        else
                        {
                            Send(state, ProtocolServerLogic.Serialize(new InvalidOperationException("The server could not return an answer.")));
                        }
                    }

                    if (commandTextArray.Length > 1 && !string.IsNullOrEmpty(commandTextArray[commandTextArray.Length - 1]))
                    {
                        state.StringBuilder = new StringBuilder(commandTextArray[commandTextArray.Length - 1]);
                    }
                    else
                    {
                        state.StringBuilder = new StringBuilder();
                    }

                    state.Listener.BeginReceive(state.Buffer, 0, state.BufferSize, SocketFlags.None, ReceiveCallbackV1, state);
                }
                else
                {
                    if (receivedBytes > 0)
                    {
                        state.Listener.BeginReceive(state.Buffer, 0, state.BufferSize, SocketFlags.None,
                            ReceiveCallbackV1, state);
                    }
                }
            }
            catch (NullReferenceException)
            {
                Close(state);
            }
            catch (SocketException)
            {
                Close(state);
            }
        }

        public void Send(StateObject stateObject, string msg)
        {
            Check.ArgumentNull(stateObject, nameof(stateObject));

            try
            {
                var send = Encoding.ASCII.GetBytes(msg + "\r\n");
                stateObject.Listener.BeginSend(send, 0, send.Length, SocketFlags.None, SendCallback, stateObject);
            }
            catch (SocketException)
            {
                Close(stateObject);
            }
            catch (ArgumentException)
            {
                Close(stateObject);
            }
        }

        private void SendCallback(IAsyncResult result)
        {
            var state = (StateObject)result.AsyncState;
            try
            {
                state.Listener.EndSend(result);
            }
            catch (SocketException)
            {
                Close(state);
            }
            catch (ObjectDisposedException)
            {
                Close(state);
            }
            catch (NullReferenceException)
            {
            }
            finally
            {
                // Message submitted to client
            }
        }

        private void Close(StateObject stateObject)
        {
            try
            {
                stateObject.Listener.Shutdown(SocketShutdown.Both);
                stateObject.Listener.Disconnect(true);
            }
            catch
            {
                // Closing might have disposed objects
            }
            finally
            {   
                stateObject.Listener = null;
            }
        }

        public void Dispose()
        {
            try
            {
                cancellationTokenSource.Cancel();
                waitToStopManualResetEvent.WaitOne(DisposeWaitTimeoutInMilliseconds);
            }
            catch (Exception)
            {
                // We do not mind at this point
            }
        }
    }
}