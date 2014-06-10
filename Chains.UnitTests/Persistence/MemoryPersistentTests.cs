namespace Chains.UnitTests.Persistence
{
    using Chains.Persistence;
    using Chains.UnitTests.Persistence.Classes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MemoryPersistentTests
    {
        [TestMethod]
        public void Persistence_WhenActionsAreBeenExecuted_ThenTheObjectCanBeLoadedAgain()
        {
            var store = new MemoryPersistentStore<ContextData>();

            var persistentTestContext =
                new PersistentTestContext(store)
                    .Do(new ChangeNameAction("Name 1"))
                    .Do(new ChangeAddressAction("Address 3"))
                    .Do(new ChangeAddressAction("Address 2"))
                    .Do(new ChangeNameAction("Name 2"))
                    .Do(new ChangeAddressAction("Address 1"))
                    .Do(new ChangeNameAction("Name 3"));

            var persistentTestContext2 = new PersistentTestContext(
                new ContextData
                {
                    Id = persistentTestContext.Data.Id
                },
                store);

            Assert.AreEqual("Name 3", persistentTestContext2.Data.Name);
            Assert.AreEqual("Address 1", persistentTestContext2.Data.Address);
        }

        [TestMethod]
        public void Persistence_WhenAChainIsBeenDeleted_ThenTheObjectDoesNotHavePreviousValues()
        {
            var store = new MemoryPersistentStore<ContextData>();

            var persistentTestContext =
                new PersistentTestContext(store)
                    .Do(new ChangeNameAction("Name 1"))
                    .Do(new ChangeAddressAction("Address 3"))
                    .Do(new ChangeAddressAction("Address 2"))
                    .Do(new ChangeNameAction("Name 2"))
                    .Do(new ChangeAddressAction("Address 1"))
                    .Do(new ChangeNameAction("Name 3"));

            new MemoryPersistentStore<ContextData>().DeleteActionEvent(persistentTestContext.Data);

            var persistentTestContext2 = new PersistentTestContext(
                new ContextData
                {
                    Id = persistentTestContext.Data.Id
                },
                store);

            Assert.AreNotEqual("Name 3", persistentTestContext2.Data.Name);
            Assert.AreNotEqual("Address 1", persistentTestContext2.Data.Address);
            Assert.IsNull(persistentTestContext2.Data.Name);
            Assert.IsNull(persistentTestContext2.Data.Address);
        }
    }
}
