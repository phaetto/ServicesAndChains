namespace Services.Communication.Protocol
{
    using Chains.Play.Web;

    public interface IClientProtocolStack
    {
        void OpenClientConnection(Client context);

        void CloseClientConnection();

        void RetryClientConnection(Client context);

        void SendStream(string data);

        string SendAndReceiveStream(string data);
    }
}