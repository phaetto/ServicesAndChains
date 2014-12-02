namespace Services.Management.Administration.Server
{
    using System;
    using Chains.Play;

    [Serializable]
    public sealed class AdministrationData : SerializableSpecification
    {
        public DateTime StartedOn;

        public string ServerLog = string.Empty;

        public int ProcessId;

        public string RepositoryFolder;

        public string ServicesFolder;

        public string DataFolder;

        public override int DataStructureVersionNumber
        {
            get
            {
                return 1;
            }
        }
    }
}
