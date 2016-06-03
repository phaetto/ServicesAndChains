namespace Services.Communication.Tcp.Servers
{
    using System.IO;
    using System.Net.Sockets;
    using System.Text;

    public class StateObject
    {
        public const int HeaderDefaultBufferSize = 16;
        public const int BodyDefaultBufferSize = 8 * 1024;
        public const string V1Header = "TcpCommand";

        /// <summary>
        /// Spec: 
        /// SV2-----...[16]|xxxxx...[8]|...[x]
        /// 16 bytes header
        ///     3 bytes header name with version number
        ///     13 bytes with features
        ///         k : keep connection open todo
        ///         z : zip todo - module?
        ///         e : encryption enabled todo - module?
        /// 8 byte actual message size (64bit number)
        /// 8 byte binary message size (64bit number - x bytes)
        /// x byte the message in binary
        /// </summary>
        public const string V2Header = "SV2";
        public const string V2SupportRequestHeader = "Supports:SV2";

        public int ProtocolVersionSelected = 1;
        public Socket Listener = null;
        public int BufferSize = 128 * 1024;
        public byte[] Buffer = new byte[HeaderDefaultBufferSize];
        public StringBuilder StringBuilder = new StringBuilder();

        // V2
        public ProtocolFeature Features;
        public long ActualMessageSize = 0;
        public long BinaryMessageSize = 0;
        public long BinaryMessageSizeRetrieved = 0;
        public MemoryStream MemoryStream = new MemoryStream();
    }
}
