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
            OnAfterExecuteAction = OnBeforeExecute;
        }

        private void OnBeforeExecute(object action)
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

        public bool IsInChain<TYpe>()
        {
            return IsInChain(typeof(TYpe));
        }

        public bool IsInChain<TChainType, TReturnChainType>()
        {
            return IsInChain(typeof(IChainableAction<TChainType, TReturnChainType>));
        }

        public TYpe Get<TYpe>() where TYpe : class
        {
            return ActionList.FirstOrDefault(x => x is TYpe) as TYpe;
        }

        public IEnumerable<TYpe> GetAll<TYpe>() where TYpe : class
        {
            return ActionList.OfType<TYpe>().ToArray();
        }

        public IChainableAction<TChainType, TReturnChainType> Get<TChainType, TReturnChainType>()
        {
            return ActionList
                .FirstOrDefault(x => x is IChainableAction<TChainType, TReturnChainType>)
                as IChainableAction<TChainType, TReturnChainType>;
        }

        public IEnumerable<IChainableAction<TChainType, TReturnChainType>> GetAll<TChainType, TReturnChainType>()
        {
            return ActionList.OfType<IChainableAction<TChainType, TReturnChainType>>().ToArray();
        }
    }
}
