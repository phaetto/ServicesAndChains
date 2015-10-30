namespace Chains.UnitTests.Persistence
{
    using Chains.Persistence.Exceptions;
    using Chains.UnitTests.Classes;
    using Chains.UnitTests.Persistence.Classes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    class PersistentContextTest
    {
        [TestMethod]
        public void Persistence_WhenActionsAreBeenExecutedInParallel_ThenOptimisticConcurrencyErrorMustOccur()
        {
            var store = new CustomPersistentStore<ContextData>();

            var persistentTestContext =
                new PersistentTestContext(store).Do(new ChangeNameAction("Name 1"))
                                                .Do(new ChangeNameAction("Name 2"))
                                                .Do(new ChangeNameAction("Name 3"));

            // Emulates a concurrency failure
            CustomPersistentStore<ContextData>.memoryStoreDateTimes[persistentTestContext.Data.Id] =
                CustomPersistentStore<ContextData>.memoryStoreDateTimes[persistentTestContext.Data.Id].AddSeconds(10);

            Test.Throws<DataIntegrityViolationException>(() => persistentTestContext.Do(new ChangeNameAction("Another Name")));

            Assert.AreEqual("Name 3", persistentTestContext.Data.Name);
        }
    }
}
