namespace Chains.Persistence
{
    using System;
    using Chains;

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
            this.OnAfterExecuteAction = OnAfterExecute;

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

        public TData Data { get; private set; }

        private void OnAfterExecute(object action)
        {
            persistentStore.SaveSnapshot(Data);
        }
    }
}
