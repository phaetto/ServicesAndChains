namespace Services.Communication.Tcp.Servers
{
    using System;
    using Chains.Play;
    using SuperSocket.SocketBase.Command;
    using SuperSocket.SocketBase.Protocol;

    public class TcpCommand : StringCommandBase<TcpSession>
    {
        public override void ExecuteCommand(TcpSession session, StringRequestInfo commandInfo)
        {
            if (string.IsNullOrWhiteSpace(commandInfo.Body))
            {
                session.Send(session.Server.ProtocolServerLogic.Serialize(new InvalidOperationException("The command body was empty.")));

                return;
            }

            var response = session.Server.ProtocolServerLogic.ReadFromStreamAndPlay(commandInfo.Body);

            if (!string.IsNullOrWhiteSpace(response))
            {
                session.Send(response);
            }
            else
            {
                session.Send(session.Server.ProtocolServerLogic.Serialize(new InvalidOperationException("The server could not return an answer.")));
            }
        }
    }
}
