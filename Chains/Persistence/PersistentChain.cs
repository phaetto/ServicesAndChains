namespace Chains.Persistence
{
    using System;

    public abstract class PersistentChain<T, TData> : Chain<T>
        where T : Chain<T>
        where TData : SerializableSpecificationWithId, new()
    {
        private readonly IPersistentStore<TData> persistentStore;

        protected PersistentChain(IPersistentStore<TData> persistentStore)
            : this(new TData
                   {
                       Id = Guid.NewGuid().ToString()
                   },
                persistentStore)
        {
        }

        protected PersistentChain(TData data, IPersistentStore<TData> persistentStore)
        {
            Data = data;
            this.persistentStore = persistentStore;

            if (!persistentStore.SnapshotExists(Data))
            {
                persistentStore.SaveSnapshot(Data);
            }
            else
            {
                var snapshot = persistentStore.LoadSnapshot(Data);
                if (snapshot != null)
                {
                    Data = snapshot;
                }
            }
        }

        protected override TReturnChainType InvokeAct<TReturnChainType>(IChainableAction<T, TReturnChainType> action)
        {
            var result = base.InvokeAct(action);

            persistentStore.SaveSnapshot(Data);

            return result;
        }

        public TData Data { get; private set; }
    }
}
