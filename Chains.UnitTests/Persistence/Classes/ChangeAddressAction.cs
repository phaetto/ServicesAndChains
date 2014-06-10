namespace Chains.UnitTests.Persistence.Classes
{
    using Chains;
    using Chains.Play;

    public sealed class ChangeAddressAction : ReproducibleWithData<ChangeAddressData>,
        IChainableAction<PersistentTestContext, PersistentTestContext>
    {
        public ChangeAddressAction(string changeAddressTo)
            : base(new ChangeAddressData
                   {
                       ChangeAddressTo = changeAddressTo
                   })
        {
        }

        public ChangeAddressAction(ChangeAddressData data)
            : base(data)
        {
        }

        public PersistentTestContext Act(PersistentTestContext context)
        {
            context.Data.Address = Data.ChangeAddressTo;
            return context;
        }
    }
}
