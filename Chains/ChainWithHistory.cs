namespace Chains
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Chains.Exceptions;

    public class ChainWithHistory<T> : Chain<T>
        where T : Chain<T>
    {
        public ChainWithHistory()
        {
            OnAfterExecuteAction = onBeforeExecute;
        }

        private void onBeforeExecute(object action)
        {
            AddToHistory(action);
        }

        protected readonly object ListLock = new object();

        protected readonly List<object> ActionList = new List<object>();

        public void ClearHistory()
        {
            lock (ListLock)
            {
                ActionList.Clear();
            }
        }

        public void AddToHistory(object action)
        {
            lock (ListLock)
            {
                ActionList.Add(action);
            }
        }

        public bool IsInChain(Type type)
        {
            Check.ArgumentNull(() => type);

            return ActionList.Any(x => x.GetType() == type);
        }

        public bool IsInChain<Type>()
        {
            return IsInChain(typeof(Type));
        }

        public bool IsInChain<ChainType, ReturnChainType>()
        {
            return IsInChain(typeof(IChainableAction<ChainType, ReturnChainType>));
        }

        public Type Get<Type>() where Type : class
        {
            return ActionList.FirstOrDefault(x => x is Type) as Type;
        }

        public IEnumerable<Type> GetAll<Type>() where Type : class
        {
            return ActionList.OfType<Type>().ToArray();
        }

        public IChainableAction<ChainType, ReturnChainType> Get<ChainType, ReturnChainType>()
        {
            return ActionList
                .FirstOrDefault(x => x is IChainableAction<ChainType, ReturnChainType>)
                as IChainableAction<ChainType, ReturnChainType>;
        }

        public IEnumerable<IChainableAction<ChainType, ReturnChainType>> GetAll<ChainType, ReturnChainType>()
        {
            return ActionList.OfType<IChainableAction<ChainType, ReturnChainType>>().ToArray();
        }
    }
}
