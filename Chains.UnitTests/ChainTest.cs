namespace Chains.UnitTests
{
    using Chains.UnitTests.Classes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ChainTest
    {
        [TestMethod]
        public void Chain_WhenDoingAction_ThenContextIsBeenUpdated()
        {
            var result =
                new ContextForTest().Do(new ActionForTest("value 1"))
                                    .Do(new ActionForTest("value 2"))
                                    .Do(new ActionForTest("value 3"));

            Assert.AreEqual(result.contextVariable, "value 3");

            result =
                result.Do(new ActionForTest("value 4"))
                      .Do(new ActionForTest("value 5"))
                      .Do(new ActionForTest("value 6"));

            Assert.AreEqual(result.contextVariable, "value 6");
        }

        [TestMethod]
        public void Do_WhenActionWithMultipleContextsIsExecuted_ThenContextIsBeenUpdated()
        {
            var result = new ContextForTest().Do<ContextForTest>(new ActionThatPlaysInTwoContexts("value"));

            Assert.IsNotNull(result);
            Assert.AreEqual("value", result.contextVariable);
        }

        [TestMethod]
        public void DoIfNotNull_WhenAChainMightReturnNull_ThenTheActionsAfterNullAreNotExecuted()
        {
            var result = new ContextForTest();

            result.Do(new ActionForTest("value 1"))
                ?.Do(new ActionForTest("value 2"))
                ?.Do(new ActionForTest("value 3"))
                ?.Do(new ActionForTestReturnsNull())
                ?.Do(new ActionForTest("value 4"))
                ?.Do(new ActionForTest("value 5"))
                ?.Do(new ActionForTest("value 6"));

            Assert.AreEqual(result.contextVariable, "value 3");
        }

        [TestMethod]
        public void DoFirstNotNull_WhenAllOptionsAreReturningNullButExecuteOneChange_ThenTheContextShouldHaveChanged()
        {
            var result = new ContextForTest();

            result.Do(new ActionForTest("0"))
                .DoFirstNotNull(
                    x => x.Do(new ActionForTest("1"))?.Do(new ActionForTestReturnsNull()),
                    x => x.Do(new ActionForTest("2"))?.Do(new ActionForTestReturnsNull()),
                    x => x.Do(new ActionForTest("3"))?.Do(new ActionForTestReturnsNull()),
                    x => x.Do(new ActionForTest("4"))?.Do(new ActionForTestReturnsNull()),
                    x => x.Do(new ActionForTest("5"))?.Do(new ActionForTestReturnsNull()));

            Assert.AreEqual("5", result.contextVariable);
        }
    }
}
