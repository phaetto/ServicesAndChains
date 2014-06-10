namespace Chains.Play.Installation
{
    using System;

    [Serializable]
    public class InstallData : SerializableSpecification
    {
        public bool Uninstall;
        public string[] Args;

        public override int DataStructureVersionNumber
        {
            get
            {
                return 1;
            }
        }
    }
}
