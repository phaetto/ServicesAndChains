namespace Chains
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return this.ActionList.Any(x => x.GetType() == type);
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
            return ActionList.FirstOrDefault(x => x as Type != null) as Type;
        }

        public IEnumerable<Type> GetAll<Type>() where Type : class
        {
            return ActionList.Where(x => x as Type != null)
                .Select(x => x as Type).ToArray();
        }

        public IChainableAction<ChainType, ReturnChainType> Get<ChainType, ReturnChainType>()
        {
            return this.ActionList
                .FirstOrDefault(x => x as IChainableAction<ChainType, ReturnChainType> != null)
                as IChainableAction<ChainType, ReturnChainType>;
        }

        public IEnumerable<IChainableAction<ChainType, ReturnChainType>> GetAll<ChainType, ReturnChainType>()
        {
            return this.ActionList.Where(x => x as IChainableAction<ChainType, ReturnChainType> != null)
                .Select(x => x as IChainableAction<ChainType, ReturnChainType>)
                .ToArray();
        }
    }
}
