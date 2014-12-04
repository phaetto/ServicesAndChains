namespace Chains.UnitTests
{
    using System;
    using Chains.Play;
    using Chains.UnitTests.Classes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ExecutionChainTest
    {
        [TestMethod]
        public void ExecutionChain_WhenStepsAreBeenProvided_ThenPlayerPlaysThemAndReturnsLastValue()
        {
            var player =
                new ExecutionChain(new ContextForTest()).Do(new ExecuteAction(new ActionForTest("value 1")))
                                                            .Do(new ExecuteAction(new ActionForTest("value 2")))
                                                            .Do(new ExecuteAction(new ActionForTest("value 3")));

            var result = (ContextForTest)player.CurrentContext;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.contextVariable, "value 3");
        }

        [TestMethod]
        public void ExecutionChain_WhenStepsAreBeenProvidedAsTypes_ThenPlayerPlaysThemAndReturnsLastValue()
        {
            var fullName = typeof(ActionForTest).FullName;
            var qualifiedName = typeof(ActionForTest).AssemblyQualifiedName;

            var player =
                new ExecutionChain(new ContextForTest()).Do(new ExecuteAction(qualifiedName, "value 1"))
                                                            .Do(new ExecuteAction(fullName, "value 2"))
                                                            .Do(new ExecuteAction(fullName, "value 3"));

            var result = (ContextForTest)player.CurrentContext;

            Assert.AreEqual(result.contextVariable, "value 3");
        }

        [TestMethod]
        public void ExecutionChain_WhenStepsAreBeenProvidedAsTypesAndDSpec_ThenPlayerPlaysThemAndReturnsLastValue()
        {
            var fullName = typeof(ActionForTest).FullName;
            var qualifiedName = typeof(ActionForTest).AssemblyQualifiedName;
            var spec = new ExecutableActionSpecification
                              {
                                  Type = typeof(ReproducibleTestAction).FullName,
                                  Data = new ReproducibleTestData { ChangeToValue = "changed value" },
                                  DataType = typeof(ReproducibleTestData).FullName
                              };

            var player =
                new ExecutionChain(new ContextForTest()).Do(new ExecuteAction(qualifiedName, "value 1"))
                                                            .Do(new ExecuteAction(fullName, "value 2"))
                                                            .Do(new ExecuteAction(fullName, "value 3"));

            player.Do(new ExecuteActionFromSpecification(spec));

            var result = (ContextForTest)player.CurrentContext;

            Assert.AreEqual(result.contextVariable, "changed value");
        }

        [TestMethod]
        public void ExecutionChain_WhenSpecIsBeenUsed_ThenItCanReproduceTheItem()
        {
            var testAction = new ReproducibleTestAction(
                new ReproducibleTestData
                {
                    ChangeToValue = "new value"
                });

            var player = new ExecutionChain(new ContextForTest());
            player.Do(new ExecuteActionFromSpecification(testAction.GetInstanceSpec()));

            var result = (ContextForTest)player.CurrentContext;

            Assert.AreEqual(result.contextVariable, "new value");
        }

        [TestMethod]
        public void ExecutionChain_WhenActionWithMultipleContextsPlays_ThenPlayerIsNotSupportingThisAction()
        {
            Exception exception = null;

            try
            {
                new ExecutionChain(new ContextForTest()).Do(
                    new ExecuteAction(new ActionThatPlaysInTwoContexts("value")));
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            Assert.IsNotNull(exception);
            Assert.IsInstanceOfType(exception, typeof(NotSupportedException));
        }
    }
}
