﻿namespace Chains.UnitTests.Classes
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

        public override ReproducibleTestData Act(ContextForTest context)
        {
            context.ContextVariable = Data.ChangeToValue;
            return Data;
        }
    }
}
