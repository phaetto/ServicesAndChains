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
            context.ContextVariable = ValueToChangeTo;
            return context;
        }
    }
}
