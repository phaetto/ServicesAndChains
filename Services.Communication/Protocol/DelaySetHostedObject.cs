namespace Services.Communication.Protocol
{
    using Chains;
    using Chains.Play;

    public sealed class DelaySetHostedObject : IChainableAction<ServerConnectionContext, ServerConnectionContext>
    {
        private readonly object hostedObject;

        private readonly string hostedObjectType;

        public DelaySetHostedObject(object hostedObject)
        {
            this.hostedObject = hostedObject;
        }

        public DelaySetHostedObject(string hostedObjectType)
        {
            this.hostedObjectType = hostedObjectType;
        }

        public ServerConnectionContext Act(ServerConnectionContext context)
        {
            if (!string.IsNullOrWhiteSpace(hostedObjectType))
            {
                context.ServerProtocolStack.ProtocolServerLogic.ReplayChain = new ExecutionChain(hostedObjectType);
            }
            else
            {
                context.ServerProtocolStack.ProtocolServerLogic.ReplayChain = new ExecutionChain(hostedObject);
            }

            return context;
        }
    }
}
