namespace Chains.UnitTests.Persistence.Classes
{
    using Chains;
    using Chains.Play;

    public sealed class ChangeNameAction : ReproducibleWithData<ChangeNameData>,
        IChainableAction<PersistentTestContext, PersistentTestContext>
    {
        public ChangeNameAction(string changeNameTo)
            : base(new ChangeNameData
                   {
                       ChangeNameTo = changeNameTo
                   })
        {
        }

        public ChangeNameAction(ChangeNameData data)
            : base(data)
        {
        }

        public PersistentTestContext Act(PersistentTestContext context)
        {
            context.Data.Name = Data.ChangeNameTo;
            return context;
        }
    }
}
