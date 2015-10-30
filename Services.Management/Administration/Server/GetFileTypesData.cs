namespace Services.Management.Administration.Server
{
    using System;
    using Chains.Play;

    [Serializable]
    public class GetFileTypesData : SerializableSpecification
    {
        public string ServiceName { get; set; }
        public int Version { get; set; }
        public string File { get; set; }

        public override int DataStructureVersionNumber => 1;
    }
}
