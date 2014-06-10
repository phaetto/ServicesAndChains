namespace Services.Communication.DataStructures.NameValue
{
    using System;
    using Chains.Play;

    [Serializable]
    public sealed class KeyValueData : SerializableSpecification
    {
        public string Key;

        public string Value;

        public override int DataStructureVersionNumber
        {
            get
            {
                return 1;
            }
        }
    }
}
