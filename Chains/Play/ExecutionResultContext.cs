namespace Chains.Play
{
    public sealed class ExecutionResultContext : Chain<ExecutionResultContext>
    {
        public object LastExecutedAction;

        public object Result;

        public ExecutableActionSpecification ToSpecification()
        {
            return new ExecutableActionSpecification
                   {
                       DataType = Result.GetType().FullName,
                       Data = Result,
                   };
        }
    }
}
