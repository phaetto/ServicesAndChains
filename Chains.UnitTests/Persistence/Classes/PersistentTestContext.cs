namespace Chains.UnitTests.Persistence.Classes
{
    using Chains.Persistence;

    public sealed class PersistentTestContext : IncrementallyPersistentChain<PersistentTestContext, ContextData>
    {
        public PersistentTestContext(IPersistentStore<ContextData> store) : base(store)
        {
        }

        public PersistentTestContext(ContextData data, IPersistentStore<ContextData> store)
            : base(data, store)
        {
        }
    }
}
