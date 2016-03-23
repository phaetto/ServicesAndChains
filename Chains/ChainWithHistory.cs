namespace Chains
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Chains.Exceptions;

    public class ChainWithHistory<T> : Chain<T>
        where T : Chain<T>
    {
        protected override TReturnChainType InvokeAct<TReturnChainType>(IChainableAction<T, TReturnChainType> action)
        {
            AddToHistory(action);
            return base.InvokeAct(action);
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
            Check.ArgumentNull(type, nameof(type));

            return ActionList.Any(x => x.GetType() == type);
        }

        public bool IsInChain<TActionType>()
        {
            return IsInChain(typeof(TActionType));
        }

        public bool IsInChain<TChainType, TReturnChainType>()
        {
            return IsInChain(typeof(IChainableAction<TChainType, TReturnChainType>));
        }

        public TActionType Get<TActionType>() where TActionType : class
        {
            return ActionList.FirstOrDefault(x => x is TActionType) as TActionType;
        }

        public IEnumerable<TActionType> GetAll<TActionType>() where TActionType : class
        {
            return ActionList.OfType<TActionType>().ToArray();
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
