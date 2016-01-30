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

        public IEnumerable<TReturnChainType> Do<TReturnChainType>(
            IEnumerable<IChainableAction<T, TReturnChainType>> actions)
        {
            Check.ArgumentNull(() => actions);

            foreach (var action in actions)
            {
                OnBeforeExecuteAction?.Invoke(action);

                var result = action.Act((T) this);

                OnAfterExecuteAction?.Invoke(action);

                yield return result;
            }
        }

        public IEnumerable<Task<TReturnChainType>> Do<TReturnChainType>(
            IEnumerable<IChainableAction<T, Task<TReturnChainType>>> actions)
        {
            Check.ArgumentNull(() => actions);

            foreach (var action in actions)
            {
                OnBeforeExecuteAction?.Invoke(action);

                var result = action.Act((T)this).ContinueWith(
                    x =>
                    {
                        OnAfterExecuteAction?.Invoke(action);

                        return x.Result;
                    });

                yield return result;
            }
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

        public Task<TReturnChainType> DoAsync<TReturnChainType>(
            IEnumerable<IChainableAction<T, TReturnChainType>> actions)
        {
            Check.ArgumentNull(() => actions);

            return Task.Factory.StartNew(() =>
                {
                    var results = Do(actions);
                    var lastResult = default(TReturnChainType);
                    foreach (var result in results)
                    {
                        lastResult = result;
                    }

                    return lastResult;
                });
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
