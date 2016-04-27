namespace Services.Communication.Tcp.Servers
{
    using System.Net.Sockets;
    using System.Text;

    public class StateObject
    {
        public const int HeaderDefaultBufferSize = 16;
        public const string V1Header = "TcpCommand";
        public const string V2Header = "SV2";

        public Socket Listener = null;
        public int BufferSize = 128 * 1024;
        public byte[] Buffer = new byte[HeaderDefaultBufferSize];
        public StringBuilder StringBuilder = new StringBuilder();
    }
}
