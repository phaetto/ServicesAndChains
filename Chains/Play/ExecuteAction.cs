namespace Chains.Play
{
    using System;
    using Chains.Exceptions;

    public sealed class ExecuteAction : IChainableAction<ExecutionChain, ExecutionChain>
    {
        private readonly dynamic chainableAction;

        public ExecuteAction(dynamic chainableAction)
        {
            Check.ArgumentNull(chainableAction, nameof(chainableAction));

            this.chainableAction = chainableAction;
        }

        public ExecuteAction(string chainableActionTypeName, params object[] arguments)
        {
            Check.ArgumentNullOrEmpty(chainableActionTypeName, nameof(chainableActionTypeName));

            chainableAction = ExecutionChain.CreateObjectWithParameters(chainableActionTypeName, arguments);
        }

        public ExecutionChain Act(ExecutionChain context)
        {
            context.Do(new ExecuteActionAndGetResult(chainableAction));

            return context;
        }
    }

    public sealed class ExecuteAction<T> : IChainableAction<ExecutionChain, T>
    {
        private readonly dynamic chainableAction;

        public ExecuteAction(dynamic chainableAction)
        {
            Check.ArgumentNull(chainableAction, nameof(chainableAction));

            this.chainableAction = chainableAction;
        }

        public ExecuteAction(string chainableActionTypeName, params object[] arguments)
        {
            Check.ArgumentNullOrEmpty(chainableActionTypeName, nameof(chainableActionTypeName));

            chainableAction = ExecutionChain.CreateObjectWithParameters(chainableActionTypeName, arguments);
        }

        public T Act(ExecutionChain context)
        {
            var resultContext = context.Do(new ExecuteActionAndGetResult(chainableAction));

            if (Equals(resultContext.Result, null))
            {
                return default(T);
            }

            return (T)Convert.ChangeType(resultContext.Result, typeof(T));
        }
    }
}
