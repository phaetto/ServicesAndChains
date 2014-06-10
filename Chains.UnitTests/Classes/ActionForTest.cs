namespace Chains.UnitTests.Classes
{
    public class ActionForTest: IChainableAction<ContextForTest, ContextForTest>
    {
        public readonly string ValueToChangeTo;

        public ActionForTest(string valueToChangeTo)
        {
            ValueToChangeTo = valueToChangeTo;
        }

        public ContextForTest Act(ContextForTest context)
        {
            context.contextVariable = ValueToChangeTo;
            return context;
        }
    }
}
