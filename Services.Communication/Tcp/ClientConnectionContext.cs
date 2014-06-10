namespace Services.Communication.Tcp
{
    using System;
    using System.Net.Sockets;
    using Chains;
    using Chains.Play.Web;

    [Obsolete]
    public sealed class ClientConnectionContext : ChainWithParent<ClientConnectionContext, Client>, IDisposable
    {
        public Socket ClientSocket { get; set; }

        public ClientConnectionContext(Client chain)
            : base(chain)
        {   
        }

        public void Close()
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

        public void Dispose()
        {
            Close();
        }
    }
}
