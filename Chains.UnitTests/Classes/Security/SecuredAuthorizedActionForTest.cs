namespace Chains.UnitTests.Classes.Security
{
    using Chains.Play;
    using Chains.Play.Security;

    public class SecuredAuthorizableActionForTest : ReproducibleWithData<ReproducibleTestData>, IChainableAction<SecuredContextForTest, SecuredContextForTest>, IAuthorizableAction
    {
        public string Session { get; set; }

        public SecuredAuthorizableActionForTest(ReproducibleTestData data)
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
