namespace Chains.Play
{
    public abstract class RemotableActionWithSerializableData<TSend, TReceived, TChain> : ReproducibleWithSerializableData<TSend>,
        IChainableAction<TChain, TChain>,
        IRemotable
    {
        public ExecutableActionSpecification ReturnData { get; private set; }

        protected RemotableActionWithSerializableData(TSend data)
            : base(data)
        {
        }

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

    public abstract class RemotableActionWithSerializableData<TReceived, TChain> : Reproducible,
        IChainableAction<TChain, TChain>,
        IRemotable
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