namespace Services.Communication.DataStructures.NameValue
{
    using Chains;
    using Chains.Play;

    public sealed class AddKeyValue : ReproducibleWithData<KeyValueData>, IChainableAction<HashContext, HashContext>
    {
        public AddKeyValue(KeyValueData data)
            : base(data)
        {
        }

        public HashContext Act(HashContext context)
        {
            context.KeyValueStore.Add(Data.Key, Data.Value);

            return context;
        }
    }
}
