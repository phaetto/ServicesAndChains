namespace Chains.Play
{
    public abstract class RemotableAction<TReceived, TChain> : Reproducible,
        IChainableAction<TChain, TChain>,
        IRemotable
        where TReceived : SerializableSpecification, new()
    {
        public ExecutableActionSpecification ReturnData { get; private set; }

        public TChain Act(TChain context)
        {
            var result = ActRemotely(context);
            ReturnData = new ExecutableActionSpecification
                         {
                             Data = result,
                             DataType = result.GetType().FullName
                         };
            return context;
        }

        protected abstract TReceived ActRemotely(TChain context);
    }
}
