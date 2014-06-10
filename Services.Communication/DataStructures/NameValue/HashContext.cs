namespace Services.Communication.DataStructures.NameValue
{
    using System.Collections.Generic;
    using Chains;

    public sealed class HashContext : Chain<HashContext>
    {
        public readonly Dictionary<string, string> KeyValueStore = new Dictionary<string, string>();
    }
}
