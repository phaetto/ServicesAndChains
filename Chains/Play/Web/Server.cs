namespace Chains.Play.Web
{
    public class ServerHost : ChainWithParent<ServerHost, Client>
    {
        public ServerHost(string hostname, int port, string path)
            : base(new Client(hostname, port, path))
        {
        }

        public ServerHost(string hostname, int port)
            : base(new Client(hostname, port))
        {
        }

        public ServerHost(Client chain)
            : base(chain)
        {
        }
    }
}
