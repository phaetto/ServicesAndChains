namespace Services.Communication.Tcp
{
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using Chains.Play.Web;
    using Services.Communication.Protocol;
    using ProtocolType = System.Net.Sockets.ProtocolType;

    public class TcpClientProtocolStack : IClientProtocolStack
    {
        private Socket ClientSocket;

        public void OpenClientConnection(Client context)
        {
            var hostname = context.Hostname == IPAddress.Any.ToString()
                ? IPAddress.Loopback.ToString()
                : context.Hostname;

            ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ClientSocket.Connect(hostname, context.Port);
        }

        public void CloseClientConnection()
        {
            if (ClientSocket == null)
            {
                return;
            }

            try
            {
                ClientSocket.Close();
                ClientSocket.Dispose();
            }
            finally
            {
                ClientSocket = null;
            }
        }

        public void RetryClientConnection(Client context)
        {
            CloseClientConnection();

            OpenClientConnection(context);
        }

        public void SendStream(string data)
        {
            InternalSendStream(data);
        }

        public string SendAndReceiveStream(string data)
        {
            return InternalSendAndReceiveStream(data);
        }

        private void InternalSendStream(string data, string command = "TcpCommandWithNoResponse")
        {
            if (!(ClientSocket.Poll(0, SelectMode.SelectWrite) && ClientSocket.Available == 0))
            {
                throw new SocketException((int)SocketError.ConnectionAborted);
            }

            using (var socketStream = new NetworkStream(ClientSocket))
            {
                socketStream.ReadTimeout = 60 * 1000;
                socketStream.WriteTimeout = 60 * 1000;

                using (var writer = new StreamWriter(socketStream, Encoding.ASCII))
                {
                    writer.Write($"{command} {data}\r\n");
                    writer.Flush();
                }
            }
        }

        private string InternalSendAndReceiveStream(string data, string command = "TcpCommand")
        {
            if (ClientSocket == null || !(ClientSocket.Poll(0, SelectMode.SelectWrite) && ClientSocket.Available == 0))
            {
                throw new SocketException((int)SocketError.ConnectionAborted);
            }

            using (var socketStream = new NetworkStream(ClientSocket))
            {
                socketStream.ReadTimeout = 60 * 1000;
                socketStream.WriteTimeout = 60 * 1000;

                using (var writer = new StreamWriter(socketStream, Encoding.ASCII))
                {
                    writer.Write($"{command} {data}\r\n");
                    writer.Flush();

                    using (var reader = new StreamReader(socketStream, Encoding.ASCII))
                    {
                        var line = reader.ReadLine();
                        return line;
                    }
                }
            }
        }
    }
}
