namespace Chains.Play
{
    public sealed class ExecuteActionFromSpecification : IChainableAction<ExecutionChain, ExecutionResultContext>
    {
        private readonly ExecutableActionSpecification actionSpecification;

        public ExecuteActionFromSpecification(ExecutableActionSpecification actionSpecification)
        {
            this.actionSpecification = actionSpecification;
        }

        public ExecutionResultContext Act(ExecutionChain context)
        {
            return context.Do(new ExecuteActionAndGetResult(actionSpecification.CreateFromSpec()));
        }
    }
}
