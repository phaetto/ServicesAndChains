namespace Chains.UnitTests
{
    using System.Collections.Concurrent;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;
    using Chains.UnitTests.Classes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ChainIEnumerableTest
    {
        private const int TimeBetweenValueUpdates = 10;

        [TestMethod]
        [SuppressMessage("ReSharper", "UnusedVariable")]
        public void Chain_WhenInfiniteListIsProvided_ThenSuccesfullyAppliesItems()
        {
            var blockingColection = new BlockingCollection<ActionForTest>(new ConcurrentQueue<ActionForTest>(), 100);
            var contextForTest = new ContextForTest();

            Task.Delay(10).ContinueWith(x =>
                {
                    blockingColection.Add(new ActionForTest("Value 1"));
                }).ContinueWith(x => Task.Delay(TimeBetweenValueUpdates).ContinueWith(y =>
                {
                    blockingColection.Add(new ActionForTest("Value 2"));
                })).ContinueWith(x => Task.Delay(TimeBetweenValueUpdates).ContinueWith(y =>
                {
                    blockingColection.Add(new ActionForTest("Value 3"));
                })).ContinueWith(x => Task.Delay(TimeBetweenValueUpdates).ContinueWith(y =>
                {
                    // Signals that there are no more items
                    blockingColection.CompleteAdding();
                }));

            var infiniteStream = contextForTest.Do(blockingColection.GetConsumingEnumerable());

            // This will block until there are no more items in the enum
            foreach (var updatedContext in infiniteStream)
            {
                // Called each time an item gets in
            }

            Assert.AreEqual("Value 3", contextForTest.contextVariable);
        }

        [TestMethod]
        public async Task Chain_WhenInfiniteListIsProvidedAsync_ThenSuccesfullyAppliesItems()
        {
            var blockingColection = new BlockingCollection<ActionForTest>(new ConcurrentQueue<ActionForTest>(), 100);

            var contextForTest = new ContextForTest();

            // This will produce an infinite stream and enumerate it
            var task = contextForTest.DoAsync(blockingColection.GetConsumingEnumerable());

            blockingColection.Add(new ActionForTest("Value 1"));
            blockingColection.Add(new ActionForTest("Value 2"));
            blockingColection.Add(new ActionForTest("Value 3"));

            blockingColection.CompleteAdding();
            await task;

            Assert.AreEqual("Value 3", contextForTest.contextVariable);
        }

        [TestMethod]
        public void Chain_WhenInfiniteListIsProvidedAsync_ThenSuccesfullyAppliesItemsInEventLikeMannerRightAfterTheStreamItem()
        {
            var blockingColection = new BlockingCollection<ActionForTest>(new ConcurrentQueue<ActionForTest>(), 100);

            var contextForTest = new ContextForTest();

            // This will produce an infinite stream and enumerate it
            contextForTest.DoAsync(blockingColection.GetConsumingEnumerable());

            blockingColection.Add(new ActionForTest("Value 1"));

            Thread.Sleep(TimeBetweenValueUpdates);
            Assert.AreEqual("Value 1", contextForTest.contextVariable);

            blockingColection.Add(new ActionForTest("Value 2"));

            Thread.Sleep(TimeBetweenValueUpdates);
            Assert.AreEqual("Value 2", contextForTest.contextVariable);

            blockingColection.Add(new ActionForTest("Value 3"));

            Thread.Sleep(TimeBetweenValueUpdates);
            Assert.AreEqual("Value 3", contextForTest.contextVariable);
        }
    }
}
