namespace Services.Management.Administration.Update
{
    using System;
    using Chains.Play;

    [Serializable]
    public class UpdateWorkUnitData : SerializableSpecification
    {
        public string ServiceName;
        public string UpdateFolderOrFile;

        public override int DataStructureVersionNumber => 1;
    }
}
