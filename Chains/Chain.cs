namespace Chains
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Chains.Exceptions;

    public class Chain<T> : AbstractChain
        where T : Chain<T>
    {
        public TReturnChainType Do<TReturnChainType>(
            IChainableAction<T, TReturnChainType> action)
        {
            Check.ArgumentNull(() => action);

            OnBeforeExecuteAction?.Invoke(action);

            var result = action.Act((T)this);

            OnAfterExecuteAction?.Invoke(action);

            return result;
        }

        public Task<TReturnChainType> Do<TReturnChainType>(
            IChainableAction<T, Task<TReturnChainType>> action)
        {
            Check.ArgumentNull(() => action);

            OnBeforeExecuteAction?.Invoke(action);

            var result = action.Act((T)this).ContinueWith(
                x =>
                {
                    OnAfterExecuteAction?.Invoke(action);

                    return x.Result;
                });

            return result;
        }

        public Task<TReturnChainType> DoAsync<TReturnChainType>(
            IChainableAction<T, TReturnChainType> action)
        {
            Check.ArgumentNull(() => action);

            OnBeforeExecuteAction?.Invoke(action);

            var resultTask = Task.Factory.StartNew(
                () =>
                {
                    var result = action.Act((T)this);

                    OnAfterExecuteAction?.Invoke(action);

                    return result;
                });

            return resultTask;
        }

        public TReturnChainType DoFirst<TReturnChainType>(
            Func<TReturnChainType, bool> condition,
            params IChainableAction<T, TReturnChainType>[] actions)
        {
            Check.ArgumentNull(() => condition);
            Check.ArgumentNull(() => actions);

            foreach (var action in actions)
            {
                OnBeforeExecuteAction?.Invoke(action);

                var actionResult = action.Act((T)this);

                if (!condition(actionResult))
                {
                    continue;
                }

                OnAfterExecuteAction?.Invoke(action);

                return actionResult;
            }

            return default(TReturnChainType);
        }

        public TReturnChainType DoFirst<TReturnChainType>(
            Func<TReturnChainType, bool> condition,
            params Func<T, TReturnChainType>[] actions)
        {
            Check.ArgumentNull(() => condition);
            Check.ArgumentNull(() => actions);

            foreach (var action in actions)
            {
                var actionResult = action((T)this);
                if (condition(actionResult))
                {
                    return actionResult;
                }
            }

            return default(TReturnChainType);
        }

        public TReturnChainType DoFirstNotNull<TReturnChainType>(params IChainableAction<T, TReturnChainType>[] actions)
        {
            return DoFirst(x => !Equals(x, null), actions);
        }

        public TReturnChainType DoFirstNotNull<TReturnChainType>(params Func<T, TReturnChainType>[] actions)
        {
            return DoFirst(x => !Equals(x, null), actions);
        }

        public IEnumerable<TReturnChainType> DoParallelFor<TReturnChainType>(
            int from,
            int to,
            IChainableAction<T, TReturnChainType> action)
        {
            Check.ArgumentOutOfRange(from < 0, () => from, "Should be larger than -1.");
            Check.ArgumentOutOfRange(to < 1, () => to, "Should be larger than 0");
            Check.ArgumentOutOfRange(to < from, () => to, "To should be larger than From.");
            Check.ArgumentNull(() => action);

            OnBeforeExecuteAction?.Invoke(action);

            var results = new TReturnChainType[to - from];

            Parallel.For(
                from,
                to,
                (number, state) =>
                {
                    results[number] = action.Act((T)this);

                    OnAfterExecuteAction?.Invoke(action);
                });

            return results;
        }

        public IEnumerable<TReturnChainType> DoParallelFor<TReturnChainType>(
            IChainableAction<T, TReturnChainType>[] actions)
        {
            Check.ArgumentNull(() => actions);

            if (OnBeforeExecuteAction != null)
            {
                foreach (var action in actions)
                {
                    OnBeforeExecuteAction(action);
                }
            }

            var results = new TReturnChainType[actions.Length];

            Parallel.For(
                0,
                actions.Length,
                (number, state) =>
                {
                    results[number] = actions[number].Act((T)this);

                    OnAfterExecuteAction?.Invoke(actions[number]);
                });

            return results;
        }

        public T Aggregate<TContextType>(TContextType context, IChainableAction<TContextType, T> action)
            where TContextType : Chain<TContextType>
        {
            Check.ArgumentNull(() => action);

            var aggregatable = this as IAggreggatable<T>;

            Check.ConditionNotSupported(aggregatable == null,
                $"Aggregation is not supported from the type {GetType().FullName}");

            aggregatable.AggregateToThis(context.Do(action));

            return (T)this;
        }
    }
}
