namespace Chains.Play
{
    public abstract class RemotableActionWithData<TSend, TReceived, TChain> : ReproducibleWithData<TSend>,
        IChainableAction<TChain, TReceived>,
        IRemotable
        where TSend : SerializableSpecification, new()
    {
        protected RemotableActionWithData(TSend data)
            : base(data)
        {
        }

        public abstract TReceived Act(TChain context);
    }
}