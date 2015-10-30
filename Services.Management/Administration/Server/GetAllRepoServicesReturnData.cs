namespace Services.Management.Administration.Server
{
    using System;
    using System.Collections.Generic;
    using Chains.Play;

    [Serializable]
    public class GetAllRepoServicesReturnData : SerializableSpecification
    {
        public Dictionary<string, List<RepoServicesData>> RepoServices;

        public override int DataStructureVersionNumber => 1;
    }
}
