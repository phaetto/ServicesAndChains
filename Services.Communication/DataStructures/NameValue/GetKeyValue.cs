namespace Services.Communication.DataStructures.NameValue
{
    using Chains.Play;

    public sealed class GetKeyValue : RemotableActionWithData<KeyData, KeyValueData, HashContext>
    {
        public GetKeyValue(KeyData data)
            : base(data)
        {
        }

        protected override KeyValueData ActRemotely(HashContext context)
        {
            return new KeyValueData
                   {
                       Value = context.KeyValueStore[Data.Key],
                       Key = Data.Key
                   };
        }
    }
}
