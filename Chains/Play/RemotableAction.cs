namespace Chains.Play
{
    public abstract class RemotableAction<TReceived, TChain> : Reproducible,
        IChainableAction<TChain, TReceived>,
        IRemotable
        where TReceived : SerializableSpecification, new()
    {
        public abstract TReceived Act(TChain context);
    }
}
