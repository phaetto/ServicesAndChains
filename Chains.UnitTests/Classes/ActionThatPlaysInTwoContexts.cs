namespace Chains.UnitTests.Classes
{
    public sealed class ActionThatPlaysInTwoContexts : 
        IChainableAction<ContextForTest, ContextForTest>,
        IChainableAction<ContextForTest2, ContextForTest2>
    {
        public readonly string ValueToChangeTo;

        public ActionThatPlaysInTwoContexts(string valueToChangeTo)
        {
            ValueToChangeTo = valueToChangeTo;
        }

        public ContextForTest Act(ContextForTest context)
        {
            context.contextVariable = ValueToChangeTo;
            return context;
        }

        public ContextForTest2 Act(ContextForTest2 context)
        {
            context.contextVariable = ValueToChangeTo;
            return context;
        }
    }
}
