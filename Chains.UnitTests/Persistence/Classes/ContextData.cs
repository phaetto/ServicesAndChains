namespace Chains.UnitTests.Persistence.Classes
{
    using System;
    using Chains.Persistence;

    [Serializable]
    public class ContextData : SerializableSpecificationWithId
    {
        public override int DataStructureVersionNumber => 1;

        public string Name { get; set; }

        public string Address { get; set; }

        public string Company { get; set; }

        public int CustomerNumber { get; set; }
    }
}
