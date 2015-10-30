namespace Services.Management.Administration.Server
{
    using System;
    using System.Collections.Generic;
    using Chains.Play;

    [Serializable]
    public class GetReportedDataReturnData : SerializableSpecification
    {
        public Dictionary<string, WorkUnitReportData> Reports;

        public override int DataStructureVersionNumber => 1;
    }
}
