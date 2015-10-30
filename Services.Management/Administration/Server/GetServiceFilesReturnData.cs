namespace Services.Management.Administration.Server
{
    using System;
    using Chains.Play;

    [Serializable]
    public class GetServiceFilesReturnData : SerializableSpecification
    {
        public string[] Files { get; set; }

        public override int DataStructureVersionNumber => 1;
    }
}
