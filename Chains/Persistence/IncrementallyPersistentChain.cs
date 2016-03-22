namespace Chains.Persistence
{
    using System;
    using Chains.Persistence.Exceptions;
    using Chains.Play;

    public abstract class IncrementallyPersistentChain<T, TData> : Chain<T>
        where T : Chain<T>
        where TData : SerializableSpecificationWithId, new()
    {
        private DateTime mediaLastUpdateTime = DateTime.MinValue;

        private readonly IPersistentStore<TData> persistentStore;

        private bool isReExecutingActionsInternally;

        public TData Data { get; private set; }

        protected IncrementallyPersistentChain(IPersistentStore<TData> persistentStore)
            : this(new TData
                   {
                       Id = Guid.NewGuid().ToString()
                   },
                persistentStore)
        {
        }

        protected IncrementallyPersistentChain(TData data, IPersistentStore<TData> persistentStore)
        {
            Data = data;
            this.persistentStore = persistentStore;

            CheckIfNeedsToBeLoaded();
        }

        protected override TReturnChainType InvokeAct<TReturnChainType>(IChainableAction<T, TReturnChainType> action)
        {
            if (!isReExecutingActionsInternally)
            {
                OnBeforeExecute(action);
            }

            var result = base.InvokeAct(action);

            if (!isReExecutingActionsInternally)
            {
                OnAfterExecute(action);
            }

            return result;
        }

        private void OnBeforeExecute(object action)
        {
            var reproducableAction = action as IReproducible;
            if (reproducableAction == null)
            {
                return;
            }

            var lastUpdatedElementtype = persistentStore.GetLastActionEventTime(Data);
            if (lastUpdatedElementtype > mediaLastUpdateTime)
            {
                throw new DataIntegrityViolationException("Racing error; the data tried to be saved have already been refreshed on the store.");
            }
        }

        private void OnAfterExecute(object action)
        {
            var reproducableAction = action as IReproducible;
            if (reproducableAction == null)
            {
                return;
            }

            persistentStore.AppendActionEvent(Data, reproducableAction.GetInstanceSpec());
            persistentStore.SaveSnapshot(Data);
            mediaLastUpdateTime = DateTime.UtcNow;
        }

        private void CheckIfNeedsToBeLoaded()
        {
            if (!persistentStore.SnapshotExists(Data))
            {
                persistentStore.CreateActionEvent(Data);
                persistentStore.SaveSnapshot(Data);
                mediaLastUpdateTime = DateTime.UtcNow;
                return;
            }

            var lastUpdatedElementtype = persistentStore.GetLastActionEventTime(Data);
            if (lastUpdatedElementtype > mediaLastUpdateTime)
            {
                var snapshot = persistentStore.LoadSnapshot(Data);
                if (snapshot != null)
                {
                    Data = snapshot;
                    mediaLastUpdateTime = lastUpdatedElementtype;

                    return;
                }

                var actions = persistentStore.LoadActionEvents(Data);

                var executionChain = new ExecutionChain(this);

                isReExecutingActionsInternally = true;

                foreach (var action in actions)
                {
                    executionChain.Do(new ExecuteActionFromSpecification(action));
                }

                persistentStore.SaveSnapshot(Data);

                isReExecutingActionsInternally = false;

                mediaLastUpdateTime = lastUpdatedElementtype;
            }
        }
    }
}
