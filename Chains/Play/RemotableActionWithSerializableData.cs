namespace Chains.Play
{
    public abstract class RemotableActionWithSerializableData<TSend, TReceived, TChain> : ReproducibleWithSerializableData<TSend>,
        IRemotableAction<TChain, TReceived>
    {
        protected RemotableActionWithSerializableData(TSend data)
            : base(data)
        {
        }

        public abstract TReceived Act(TChain context);
    }

    public abstract class RemotableActionWithSerializableData<TReceived, TChain> : Reproducible,
        IRemotableAction<TChain, TReceived>
    {
        public abstract TReceived Act(TChain context);
    }
}
