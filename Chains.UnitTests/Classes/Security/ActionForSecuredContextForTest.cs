namespace Chains.UnitTests.Classes.Security
{
    using Chains.Play;

    public class ActionForSecuredContextForTest : ReproducibleWithData<ReproducibleTestData>, IChainableAction<SecuredContextForTest, SecuredContextForTest>
    {
        public ActionForSecuredContextForTest(ReproducibleTestData data)
            : base(data)
        {
        }

        public SecuredContextForTest Act(SecuredContextForTest context)
        {
            context.contextVariable = Data.ChangeToValue;
            return context;
        }
    }
}
