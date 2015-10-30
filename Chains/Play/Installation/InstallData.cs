namespace Chains.Play.Installation
{
    using System;

    [Serializable]
    public class InstallData : SerializableSpecification
    {
        public string[] Args;

        public override int DataStructureVersionNumber => 1;
    }
}
