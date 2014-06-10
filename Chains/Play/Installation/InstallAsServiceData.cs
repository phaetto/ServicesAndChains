namespace Chains.Play.Installation
{
    using System;
    using System.ServiceProcess;

    [Serializable]
    public class InstallAsServiceData : SerializableSpecification
    {
        public string Name;
        public string Description;
        public ServiceAccount Account;
        public bool DelayAutoStart;

        public override int DataStructureVersionNumber
        {
            get
            {
                return 1;
            }
        }
    }
}
