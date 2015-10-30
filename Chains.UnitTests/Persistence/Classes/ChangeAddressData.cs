namespace Chains.UnitTests.Persistence.Classes
{
    using System;
    using Chains.Play;

    [Serializable]
    public sealed class ChangeAddressData : SerializableSpecification
    {
        public string ChangeAddressTo;

        public override int DataStructureVersionNumber => 1;
    }
}
