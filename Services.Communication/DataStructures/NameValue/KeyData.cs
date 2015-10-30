namespace Services.Communication.DataStructures.NameValue
{
    using System;
    using Chains.Play;

    [Serializable]
    public sealed class KeyData : SerializableSpecification
    {
        public string Key;

        public override int DataStructureVersionNumber => 1;
    }
}
