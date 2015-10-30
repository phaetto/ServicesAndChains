namespace Services.Management.Administration.Update
{
    using System;
    using Chains.Play;

    [Serializable]
    public class CopyFilesToFolderData : SerializableSpecification
    {
        public string File;
        public string FolderToUpdate;

        public override int DataStructureVersionNumber => 1;
    }
}
