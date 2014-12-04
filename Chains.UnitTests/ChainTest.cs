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
        public void IfNullRevert_WhenActionWithMultipleContextsIsExecuted_ThenContextIsBeenUpdated()
        {
            var result = new ContextForTest().Do<ContextForTest>(new ActionThatPlaysInTwoContexts("value"));

            Assert.IsNotNull(result);
            Assert.AreEqual("value", result.contextVariable);
        }

        [TestMethod]
        public void IfNullRevert_WhenSomethingIsNull_ThenTheLastKnownGoodInstanceIsReverted()
        {
            var result = new ContextForTest().Do(new ActionForTestReturnsNull()).IfNullRevert();

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void DoIfNotNull_WhenAChainMightReturnNull_ThenTheActionsAfterNullAreNotExecuted()
        {
            var result =
                new ContextForTest().DoIfNotNull(new ActionForTest("value 1"))
                                    .DoIfNotNull(new ActionForTest("value 2"))
                                    .DoIfNotNull(new ActionForTest("value 3"))
                                    .DoIfNotNull(new ActionForTestReturnsNull())
                                    .DoIfNotNull(new ActionForTest("value 4"))
                                    .DoIfNotNull(new ActionForTest("value 5"))
                                    .DoIfNotNull(new ActionForTest("value 6"))
                                    .IfNullRevert();

            Assert.AreEqual(result.contextVariable, "value 3");
        }

        [TestMethod]
        public void DoFirstNotNull_WhenManyOptionsAreReturningNull_ThenTheFirstThatIsNotNullShouldBePicked()
        {
            var result =
                new ContextForTest().Do(new ActionForTestReturnsNull())
                                    .IfNullRevert()
                                    .DoFirstNotNull(
                                        x => x.Do(new ActionForTest("1")).DoIfNotNull(new ActionForTestReturnsNull()),
                                        x => x.Do(new ActionForTestReturnsNull()).DoIfNotNull(new ActionForTest("2")),
                                        x =>
                                            x.DoIfNotNull(new ActionForTestReturnsNull()),
                                        x => x.Do(new ActionForTest("3")).DoIfNotNull(new ActionForTest("4")),
                                        x =>
                                        {
                                            Assert.Fail("Should never reach here");
                                            return x;
                                        },
                                        x => x.Do(new ActionForTest("5")).DoIfNotNull(new ActionForTest("6")));

            Assert.AreEqual("4", result.contextVariable);
        }

        [TestMethod]
        public void DoFirstNotNull_WhenAllOptionsAreReturningNullButExecuteOneChange_ThenTheContextShouldHaveChanged()
        {
            var result =
                new ContextForTest().Do(new ActionForTest("0"))
                                    .DoFirstNotNull(
                                        x => x.Do(new ActionForTest("1")).DoIfNotNull(new ActionForTestReturnsNull()),
                                        x => x.Do(new ActionForTest("2")).DoIfNotNull(new ActionForTestReturnsNull()),
                                        x => x.Do(new ActionForTest("3")).DoIfNotNull(new ActionForTestReturnsNull()),
                                        x => x.Do(new ActionForTest("4")).DoIfNotNull(new ActionForTestReturnsNull()),
                                        x => x.Do(new ActionForTest("5")).DoIfNotNull(new ActionForTestReturnsNull()))
                                    .IfNullRevert();

            Assert.AreEqual("5", result.contextVariable);
        }
    }
}
