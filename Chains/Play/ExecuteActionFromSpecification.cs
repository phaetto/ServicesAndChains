namespace Chains.Play
{
    public sealed class ExecuteActionFromSpecification : IChainableAction<ExecutionChain, ExecutionChain>
    {
        private readonly ExecutableActionSpecification actionSpecification;

        public ExecuteActionFromSpecification(ExecutableActionSpecification actionSpecification)
        {
            this.actionSpecification = actionSpecification;
        }

        public ExecutionChain Act(ExecutionChain context)
        {
            return context.Do(new ExecuteAction(actionSpecification.CreateFromSpec()));
        }
    }
}
