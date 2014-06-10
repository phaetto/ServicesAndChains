namespace Chains.Play
{
    using System;

    public sealed class ExecuteActionAsRemotable<T> : IChainableAction<ExecutionChain, T>
    {
        private readonly IRemotable chainableAction;

        public ExecuteActionAsRemotable(IRemotable chainableAction)
        {
            if (chainableAction == null)
            {
                throw new ArgumentNullException("chainableAction");
            }

            this.chainableAction = chainableAction;
        }

        public ExecuteActionAsRemotable(string chainableActionTypeName, params object[] arguments)
        {
            if (string.IsNullOrEmpty(chainableActionTypeName))
            {
                throw new ArgumentNullException("chainableActionTypeName");
            }

            chainableAction = (IRemotable)ExecutionChain.CreateObjectWithParameters(chainableActionTypeName, arguments);
        }

        public T Act(ExecutionChain context)
        {
            ExecuteAction.ApplyActionOnCurrentContext(context, (dynamic)chainableAction);

            context.LastExecutedAction = chainableAction;

            return (T)Convert.ChangeType(chainableAction.ReturnData.Data, typeof(T));
        }
    }
}
