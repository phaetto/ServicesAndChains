namespace Chains.UnitTests
{
    using System;
    using Chains.Play;
    using Chains.Play.Modules;
    using Chains.UnitTests.Classes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ModuleTest
    {
        [TestMethod]
        public void Module_WhenSupportsModule_ThenReturnsTrue()
        {
            var executionChain = new ExecutionChain(new ContextForTestWithModules());

            Assert.IsTrue(executionChain.Do(new ExecuteActionAsRemotable<bool>(new SupportsModule(typeof(ContextForTestWithModules).FullName))));
        }

        [TestMethod]
        public void Module_WhenSupportsClassIsTheSameAsContext_ThenReturnsTrue()
        {
            var executionChain = new ExecutionChain(new ContextForTestWithModules());

            Assert.IsTrue(executionChain.Do(new ExecuteActionAsRemotable<bool>(new SupportsModule(typeof(ContextForTest2).FullName))));
        }

        [TestMethod]
        public void Module_WhenDoesNotSupportModule_ThenReturnsFalse()
        {
            var executionChain = new ExecutionChain(new ContextForTestWithModules());

            Assert.IsFalse(executionChain.Do(new ExecuteActionAsRemotable<bool>(new SupportsModule(typeof(ContextForTest).FullName))));
        }

        [TestMethod]
        public void Module_WhenCallIsNotSupportedFromTheMainClass_ThenAModuleIsExecutedInstead()
        {
            var executionChain = new ExecutionChain(new ContextForTestWithModules());

            executionChain.Do(new ExecuteAction(new ReproducibleTestAction2(new ReproducibleTestData())));
        }

        [TestMethod]
        public void Module_WhenCallIsNotSupportedFromTheMainClassOrModules_ThenExceptionIsThrown()
        {
            Exception exception = null;

            try
            {
                var executionChain = new ExecutionChain(new ContextForTestWithModules());

                executionChain.Do(new ExecuteAction(new ReproducibleTestAction(new ReproducibleTestData())));

                Assert.Fail("Execution of action should have thrown an error.");
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
