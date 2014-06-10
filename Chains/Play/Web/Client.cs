namespace Chains.Play.Web
{
    public sealed class Client : Chain<Client>
    {
        public readonly string Hostname;
        public readonly int Port;
        public readonly string Path;

        public Client(string hostname, int port)
        {
            Hostname = hostname;
            Port = port;
        }

        public Client(string hostname, int port, string path)
        {
            Hostname = hostname;
            Port = port;
            Path = path;
        }
    }
}
