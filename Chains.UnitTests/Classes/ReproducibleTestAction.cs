namespace Chains.UnitTests.Classes
{
    using System;
    using Chains.Play;

    public class ReproducibleTestAction : RemotableActionWithData<ReproducibleTestData, ReproducibleTestData, ContextForTest>
    {
        public ReproducibleTestAction(ReproducibleTestData testData)
            : base(testData)
        {
            Data.DomainName = AppDomain.CurrentDomain.FriendlyName;
        }

        protected override ReproducibleTestData ActRemotely(ContextForTest context)
        {
            context.contextVariable = Data.ChangeToValue;
            return Data;
        }
    }
}
