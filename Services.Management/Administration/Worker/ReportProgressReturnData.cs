namespace Services.Management.Administration.Worker
{
    using System;
    using Chains.Play;

    [Serializable]
    public class ReportProgressReturnData : SerializableSpecification
    {
        public AdviceState Advice;

        public override int DataStructureVersionNumber => 1;
    }
}
