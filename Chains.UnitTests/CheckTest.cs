namespace Chains.UnitTests
{
    using System;
    using Chains.Exceptions;
    using Chains.UnitTests.Classes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CheckTest
    {
        private const string ExceptionPrefix = "exception: ";

        private const string ExceptionMessage = "Exception string triggered";

        [TestMethod]
        public void ArgumentNull_WhenValidatingNull_ThenItThrows()
        {
            Test.Throws<ArgumentNullException>(() => TestMethod(null));
        }

        [TestMethod]
        public void ArgumentNull_WhenValidationPasses_ThenDoesNotThrows()
        {
            TestMethod("do not throw man.");
        }

        [TestMethod]
        public void ArgumentNull_WhenValidatingConditionMet_ThenItThrows()
        {
            Test.Throws<ArgumentException>(() => TestMethod(ExceptionPrefix + "something here"));
        }

        private void TestMethod(string arg)
        {
            Check.ArgumentNull(() => arg);
            Check.Argument(arg.StartsWith(ExceptionPrefix), () => arg, ExceptionMessage);
        }
    }
}
