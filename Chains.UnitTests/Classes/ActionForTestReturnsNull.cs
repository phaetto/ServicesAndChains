namespace Chains.UnitTests.Classes
{
    public class ActionForTestReturnsNull: IChainableAction<ContextForTest, ContextForTest>
    {
        public ContextForTest Act(ContextForTest context)
        {
            return null;
        }
    }
}
