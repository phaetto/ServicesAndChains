namespace Chains.UnitTests.Persistence.Classes
{
    using System;
    using Chains.Play;

    [Serializable]
    public sealed class ChangeNameData : SerializableSpecification
    {
        public string ChangeNameTo;

        public override int DataStructureVersionNumber
        {
            get
            {
                return 1;
            }
        }
    }
}
