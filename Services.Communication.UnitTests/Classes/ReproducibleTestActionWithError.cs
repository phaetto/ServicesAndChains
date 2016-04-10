namespace Services.Communication.UnitTests.Classes
{
    using System;
    using System.Collections.Generic;
    using Chains;
    using Chains.Play;
    using Chains.UnitTests.Classes;

    public class ReproducibleTestActionWithError : ReproducibleWithData<ReproducibleTestData>, IChainableAction<ContextForTest2, ContextForTest2>
    {
        public const string InnerExceptionText = "Inner exception is here";
        public const string ExceptionText = "Some key has not been found";

        public ReproducibleTestActionWithError(ReproducibleTestData testData)
            : base(testData)
        {
            Data.DomainName = AppDomain.CurrentDomain.FriendlyName;
        }

        public ContextForTest2 Act(ContextForTest2 context)
        {
            throw new KeyNotFoundException(ExceptionText, new Exception(InnerExceptionText));
        }
    }
}
