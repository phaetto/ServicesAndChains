namespace Services.Management.Administration.Server
{
    using System;
    using Chains.Play;

    [Serializable]
    public class GetFileTypesReturnData : SerializableSpecification
    {
        public string[] Types { get; set; }

        public override int DataStructureVersionNumber
        {
            get
            {
                return 1;
            }
        }
    }
}
