namespace Chains.Play.Installation
{
    using System;

    [Serializable]
    public class AssemblyData : SerializableSpecification
    {
        public byte[] Bytes;

        public override int DataStructureVersionNumber
        {
            get
            {
                return 1;
            }
        }
    }
}
