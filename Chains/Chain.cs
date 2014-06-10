namespace Chains
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Chains.Play;

    public class Chain<T> : AbstractChain
        where T : Chain<T>
    {
        [ThreadStatic]
        internal static T LastKnownGoodObject;

        public ReturnChainType Do<ReturnChainType>(
            IChainableAction<T, ReturnChainType> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            if (OnBeforeExecuteAction != null)
            {
                OnBeforeExecuteAction(action);
            }

            var result = action.Act((T)this);

            LastKnownGoodObject = (T)this;

            if (OnAfterExecuteAction != null)
            {
                OnAfterExecuteAction(action);
            }

            return result;
        }

        public ReturnChainType Do<ReturnChainType>(Func<T, ReturnChainType> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            return action((T)this);
        }

        public ReturnChainType DoFirst<ReturnChainType>(
            Func<ReturnChainType, bool> condition,
            params IChainableAction<T, ReturnChainType>[] actions)
        {
            if (condition == null)
            {
                throw new ArgumentNullException("condition");
            }

            if (actions == null)
            {
                throw new ArgumentNullException("actions");
            }

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

                LastKnownGoodObject = (T)this;

                return actionResult;
            }

            return default(ReturnChainType);
        }

        public ReturnChainType DoFirst<ReturnChainType>(
            Func<ReturnChainType, bool> condition,
            params Func<T, ReturnChainType>[] actions)
        {
            if (condition == null)
            {
                throw new ArgumentNullException("condition");
            }

            if (actions == null)
            {
                throw new ArgumentNullException("actions");
            }

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
            if (from < 0)
            {
                throw new ArgumentOutOfRangeException("from", from, "From should be larger than -1.");
            }

            if (to < 1)
            {
                throw new ArgumentOutOfRangeException("to", to, "to should be larger than 0.");
            }

            if (to < from)
            {
                throw new ArgumentOutOfRangeException("to", from, "to should be larger than from.");
            }

            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

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

            LastKnownGoodObject = (T)this;

            return results;
        }

        public IEnumerable<ReturnChainType> DoParallelFor<ReturnChainType>(
            IChainableAction<T, ReturnChainType>[] actions)
        {
            if (actions == null)
            {
                throw new ArgumentNullException("action");
            }

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

            LastKnownGoodObject = (T)this;

            return results;
        }

        public ReturnChainType Decorate<ReturnChainType>(IChainableAction<T, ReturnChainType> action, Func<IChainableAction<T, ReturnChainType>, T, ReturnChainType> decorator)
            where ReturnChainType : class 
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            return decorator(action, (T)this);
        }

        public T Aggregate<ContextType>(ContextType context, IChainableAction<ContextType, T> action)
            where ContextType : Chain<ContextType>
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            var aggregatable = this as IAggreggatable<T>;
            
            if (aggregatable == null)
            {
                throw new NotSupportedException("Aggregation is not supported from the type " + this.GetType().FullName);
            }

            aggregatable.AggregateToThis(context.Do(action));

            return (T)this;
        }

        public ReturnChainType DoRemotable<ReturnChainType>(
            IChainableAction<T, T> action)
        {
            if (action is IRemotable)
            {
                this.Do(action);
                var remotable = action as IRemotable;
                return (ReturnChainType)Convert.ChangeType(remotable.ReturnData.Data, typeof(ReturnChainType));
            }

            throw new InvalidOperationException("The action must be an IRemotable object");
        }

        public TReceive DoRemotable<TReceive>(RemotableAction<TReceive, T> action)
            where TReceive : SerializableSpecification, new()
        {
            this.Do(action);

            return (TReceive)Convert.ChangeType(action.ReturnData.Data, typeof(TReceive));
        }

        public TReceive DoRemotable<TSend, TReceive>(RemotableActionWithData<TSend, TReceive, T> action)
            where TSend : SerializableSpecification, new()
        {
            this.Do(action);

            return (TReceive)Convert.ChangeType(action.ReturnData.Data, typeof(TReceive));
        }

        public TReceive DoRemotable<TSend, TReceive>(RemotableActionWithSerializableData<TSend, TReceive, T> action)
        {
            this.Do(action);

            return (TReceive)Convert.ChangeType(action.ReturnData.Data, typeof(TReceive));
        }
    }
}
