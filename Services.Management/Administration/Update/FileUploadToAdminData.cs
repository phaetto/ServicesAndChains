namespace Services.Management.Administration.Update
{
    using System;
    using Chains.Play;

    [Serializable]
    public class FileUploadToAdminData : SerializableSpecification
    {
        public string ServiceName;
        public byte[] FileData;

        public override int DataStructureVersionNumber
        {
            get
            {
                return 1;
            }
        }
    }
}
