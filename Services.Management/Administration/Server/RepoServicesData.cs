namespace Services.Management.Administration.Server
{
    using System;
    using Chains.Play;

    [Serializable]
    public class RepoServicesData : SerializableSpecification
    {
        public int Version;

        public DateTime CreatedTime;

        public override int DataStructureVersionNumber => 1;
    }
}
