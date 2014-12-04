namespace Chains.UnitTests
{
    using System.Threading.Tasks;
    using Chains.UnitTests.Classes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ChainAsyncTest
    {
        [TestMethod]
        public async Task Chain_WhenDoingActionTask_ThenContextIsBeenUpdated()
        {
            var result =
                await new ContextForTest().Do(new ActionForTestAsync("value 1"))
                        .Do(new ActionForTestAsync("value 2"))
                        .Do(new ActionForTestAsync("value 3"));

            Assert.AreEqual(result.contextVariable, "value 3");

            await result.Do(new ActionForTestAsync("value 4"))
                      .Do(new ActionForTestAsync("value 5"))
                      .Do(new ActionForTestAsync("value 6"));

            Assert.AreEqual(result.contextVariable, "value 6");
        }

        [TestMethod]
        public async Task Chain_WhenDoingActionTaskAndSyncMixed_ThenContextIsBeenUpdated()
        {
            var result =
                await new ContextForTest().Do(new ActionForTestAsync("value 1"))
                        .Do(new ActionForTest("value 2"))
                        .Do(new ActionForTestAsync("value 3"));

            Assert.AreEqual(result.contextVariable, "value 3");

            await result.Do(new ActionForTest("value 4"))
                      .Do(new ActionForTestAsync("value 5"))
                      .Do(new ActionForTest("value 6"));

            Assert.AreEqual(result.contextVariable, "value 6");
        }

        [TestMethod]
        public async Task Chain_WhenDoingActionAsync_ThenContextIsBeenUpdated()
        {
            var result =
                await new ContextForTest().DoAsync(new ActionForTest("value 1"))
                        .DoAsync(new ActionForTest("value 2"))
                        .DoAsync(new ActionForTest("value 3"));

            Assert.AreEqual(result.contextVariable, "value 3");

            await result.DoAsync(new ActionForTest("value 4"))
                      .DoAsync(new ActionForTest("value 5"))
                      .DoAsync(new ActionForTest("value 6"));

            Assert.AreEqual(result.contextVariable, "value 6");
        }

        [TestMethod]
        public async Task Chain_WhenDoingActionAsyncAndSyncMixed_ThenContextIsBeenUpdated()
        {
            var result =
                await new ContextForTest().DoAsync(new ActionForTest("value 1"))
                        .Do(new ActionForTest("value 2"))
                        .DoAsync(new ActionForTest("value 3"));

            Assert.AreEqual(result.contextVariable, "value 3");

            await result.Do(new ActionForTest("value 4"))
                      .DoAsync(new ActionForTest("value 5"))
                      .Do(new ActionForTest("value 6"));

            Assert.AreEqual(result.contextVariable, "value 6");
        }

        [TestMethod]
        public async Task Chain_WhenDoingActionAsyncAndTaskAndSyncMixed_ThenContextIsBeenUpdated()
        {
            var result =
                await new ContextForTest().DoAsync(new ActionForTest("value 1"))
                        .Do(new ActionForTestAsync("value 2"))
                        .Do(new ActionForTest("value 3"));

            Assert.AreEqual(result.contextVariable, "value 3");

            await result.Do(new ActionForTestAsync("value 4"))
                      .DoAsync(new ActionForTest("value 5"))
                      .Do(new ActionForTest("value 6"));

            Assert.AreEqual(result.contextVariable, "value 6");
        }
    }
}
