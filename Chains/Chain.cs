namespace Chains
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Chains.Exceptions;

    public class Chain<T> : AbstractChain
        where T : Chain<T>
    {
        protected virtual TReturnChainType InvokeAct<TReturnChainType>(IChainableAction<T, TReturnChainType> action)
        {
            return action.Act((T) this);
        }

        public TReturnChainType Do<TReturnChainType>(
            IChainableAction<T, TReturnChainType> action)
        {
            Check.ArgumentNull(action, nameof(action));

            var result = InvokeAct(action);

            return result;
        }

        public IEnumerable<TReturnChainType> Do<TReturnChainType>(
            IEnumerable<IChainableAction<T, TReturnChainType>> actions)
        {
            Check.ArgumentNull(actions, nameof(actions));

            // Yields the results
            return actions.Select(InvokeAct);
        }

        public Task<TReturnChainType> DoAsync<TReturnChainType>(
            IChainableAction<T, TReturnChainType> action)
        {
            Check.ArgumentNull(action, nameof(action));

            return Task.Factory.StartNew(() => InvokeAct(action));
        }

        public Task<TReturnChainType> DoAsync<TReturnChainType>(
            IEnumerable<IChainableAction<T, TReturnChainType>> actions)
        {
            Check.ArgumentNull(actions, nameof(actions));

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
            Check.ArgumentNull(condition, nameof(condition));
            Check.ArgumentNull(actions, nameof(actions));

            foreach (var action in actions)
            {
                var actionResult = InvokeAct(action);

                if (!condition(actionResult))
                {
                    continue;
                }

                return actionResult;
            }

            return default(TReturnChainType);
        }

        public TReturnChainType DoFirst<TReturnChainType>(
            Func<TReturnChainType, bool> condition,
            params Func<T, TReturnChainType>[] actions)
        {
            Check.ArgumentNull(condition, nameof(condition));
            Check.ArgumentNull(actions, nameof(actions));

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
            Check.ArgumentNull(action, nameof(action));

            var aggregatable = this as IAggreggatable<T>;

            Check.ConditionNotSupported(aggregatable == null,
                $"Aggregation is not supported from the type {GetType().FullName}");

            aggregatable.AggregateToThis(context.Do(action));

            return (T)this;
        }
    }
}
