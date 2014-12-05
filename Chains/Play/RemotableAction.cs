namespace Chains.Play
{
    public abstract class RemotableAction<TReceived, TChain> : Reproducible,
        IRemotableAction<TChain, TReceived>
        where TReceived : SerializableSpecification, new()
    {
        public abstract TReceived Act(TChain context);
    }
}
