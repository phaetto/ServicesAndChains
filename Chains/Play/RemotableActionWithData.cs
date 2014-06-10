namespace Chains.Play
{
    public abstract class RemotableActionWithData<TSend, TReceived, TChain> : ReproducibleWithData<TSend>,
        IChainableAction<TChain, TChain>,
        IRemotable
        where TSend : SerializableSpecification, new()
    {
        public ExecutableActionSpecification ReturnData { get; private set; }

        protected RemotableActionWithData(TSend data)
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
}