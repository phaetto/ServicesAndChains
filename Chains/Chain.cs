namespace Chains
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Chains.Exceptions;

    public class Chain<T> : AbstractChain
        where T : Chain<T>
    {
        public ReturnChainType Do<ReturnChainType>(
            IChainableAction<T, ReturnChainType> action)
        {
            Check.ArgumentNull(() => action);

            if (OnBeforeExecuteAction != null)
            {
                OnBeforeExecuteAction(action);
            }

            var result = action.Act((T)this);

            if (OnAfterExecuteAction != null)
            {
                OnAfterExecuteAction(action);
            }

            return result;
        }

        public Task<ReturnChainType> Do<ReturnChainType>(
            IChainableAction<T, Task<ReturnChainType>> action)
        {
            Check.ArgumentNull(() => action);

            if (OnBeforeExecuteAction != null)
            {
                OnBeforeExecuteAction(action);
            }

            var result = action.Act((T)this).ContinueWith(
                x =>
                {
                    if (OnAfterExecuteAction != null)
                    {
                        OnAfterExecuteAction(action);
                    }

                    return x.Result;
                });

            return result;
        }

        public Task<ReturnChainType> DoAsync<ReturnChainType>(
            IChainableAction<T, ReturnChainType> action)
        {
            Check.ArgumentNull(() => action);

            if (OnBeforeExecuteAction != null)
            {
                OnBeforeExecuteAction(action);
            }

            var resultTask = Task.Factory.StartNew(
                () =>
                {
                    var result = action.Act((T)this);

                    if (OnAfterExecuteAction != null)
                    {
                        OnAfterExecuteAction(action);
                    }

                    return result;
                });

            return resultTask;
        }

        public ReturnChainType DoFirst<ReturnChainType>(
            Func<ReturnChainType, bool> condition,
            params IChainableAction<T, ReturnChainType>[] actions)
        {
            Check.ArgumentNull(() => condition);
            Check.ArgumentNull(() => actions);

            foreach (var action in actions)
            {
                if (OnBeforeExecuteAction != null)
                {
                    OnBeforeExecuteAction(action);
                }

                var actionResult = action.Act((T)this);

                if (!condition(actionResult))
                {
                    continue;
                }

                if (OnAfterExecuteAction != null)
                {
                    OnAfterExecuteAction(action);
                }

                return actionResult;
            }

            return default(ReturnChainType);
        }

        public ReturnChainType DoFirst<ReturnChainType>(
            Func<ReturnChainType, bool> condition,
            params Func<T, ReturnChainType>[] actions)
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

            return default(ReturnChainType);
        }

        public ReturnChainType DoFirstNotNull<ReturnChainType>(params IChainableAction<T, ReturnChainType>[] actions)
        {
            return DoFirst(x => !Equals(x, null), actions);
        }

        public ReturnChainType DoFirstNotNull<ReturnChainType>(params Func<T, ReturnChainType>[] actions)
        {
            return DoFirst(x => !Equals(x, null), actions);
        }

        public IEnumerable<ReturnChainType> DoParallelFor<ReturnChainType>(
            int from,
            int to,
            IChainableAction<T, ReturnChainType> action)
        {
            Check.ArgumentOutOfRange(from < 0, () => from, "Should be larger than -1.");
            Check.ArgumentOutOfRange(to < 1, () => to, "Should be larger than 0");
            Check.ArgumentOutOfRange(to < from, () => to, "To should be larger than From.");
            Check.ArgumentNull(() => action);

            if (OnBeforeExecuteAction != null)
            {
                OnBeforeExecuteAction(action);
            }

            var results = new ReturnChainType[to - from];

            Parallel.For(
                from,
                to,
                (number, state) =>
                {
                    results[number] = action.Act((T)this);

                    if (OnAfterExecuteAction != null)
                    {
                        OnAfterExecuteAction(action);
                    }
                });

            return results;
        }

        public IEnumerable<ReturnChainType> DoParallelFor<ReturnChainType>(
            IChainableAction<T, ReturnChainType>[] actions)
        {
            Check.ArgumentNull(() => actions);

            if (OnBeforeExecuteAction != null)
            {
                foreach (var action in actions)
                {
                    OnBeforeExecuteAction(action);
                }
            }

            var results = new ReturnChainType[actions.Length];

            Parallel.For(
                0,
                actions.Length,
                (number, state) =>
                {
                    results[number] = actions[number].Act((T)this);

                    if (OnAfterExecuteAction != null)
                    {
                        OnAfterExecuteAction(actions[number]);
                    }
                });

            return results;
        }

        public T Aggregate<ContextType>(ContextType context, IChainableAction<ContextType, T> action)
            where ContextType : Chain<ContextType>
        {
            Check.ArgumentNull(() => action);

            var aggregatable = this as IAggreggatable<T>;

            Check.ConditionNotSupported(aggregatable == null, "Aggregation is not supported from the type " + this.GetType().FullName);

            aggregatable.AggregateToThis(context.Do(action));

            return (T)this;
        }
    }
}
