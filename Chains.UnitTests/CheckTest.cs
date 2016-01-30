namespace Chains.UnitTests
{
    using System;
    using Chains.Exceptions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CheckTest
    {
        private const string ExceptionPrefix = "exception: ";

        private const string ExceptionMessage = "Exception string triggered";

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ArgumentNull_WhenValidatingNull_ThenItThrows()
        {
            TestMethod(null);
        }

        [TestMethod]
        public void ArgumentNull_WhenValidationPasses_ThenDoesNotThrows()
        {
            TestMethod("do not throw man.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ArgumentNull_WhenValidatingConditionMet_ThenItThrows()
        {
            TestMethod(ExceptionPrefix + "something here");
        }

        private void TestMethod(string arg)
        {
            Check.ArgumentNull(() => arg);
            Check.Argument(arg.StartsWith(ExceptionPrefix), () => arg, ExceptionMessage);
        }
    }
}
