namespace Chains.UnitTests.Classes
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public static class Test
    {
        public static void Throws<T>(Action action)
        {
            try
            {
                action();
            }
            catch (Exception exception)
            {
                Assert.IsInstanceOfType(exception, typeof(T));
                return;
            }

            Assert.Fail("No exception was generated.");
        }
    }
}
