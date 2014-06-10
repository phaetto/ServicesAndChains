namespace Chains.UnitTests.Classes
{
    using System;
    using Chains.Play;

    public class ReproducibleTestAction2 : ReproducibleWithData<ReproducibleTestData>, IChainableAction<ContextForTest2, ContextForTest2>
    {
        public ReproducibleTestAction2(ReproducibleTestData testData)
            : base(testData)
        {
            Data.DomainName = AppDomain.CurrentDomain.FriendlyName;
        }

        public ContextForTest2 Act(ContextForTest2 context)
        {
            context.contextVariable = Data.ChangeToValue;

            return context;
        }
    }
}
