namespace Chains.Play
{
    public abstract class RemotableActionWithSerializableData<TSend, TReceived, TChain> : ReproducibleWithSerializableData<TSend>,
        IChainableAction<TChain, TReceived>,
        IRemotable
    {
        protected RemotableActionWithSerializableData(TSend data)
            : base(data)
        {
        }

        public abstract TReceived Act(TChain context);
    }

    public abstract class RemotableActionWithSerializableData<TReceived, TChain> : Reproducible,
        IChainableAction<TChain, TReceived>,
        IRemotable
    {
        public abstract TReceived Act(TChain context);
    }
}