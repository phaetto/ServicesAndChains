namespace Services.Communication.Protocol
{
    using Chains.Play.Web;

    public interface IServerProtocolStack
    {
        void OpenServerConnection(ServerHost context);

        void CloseServerConnection();
    }
}