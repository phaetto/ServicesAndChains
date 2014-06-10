namespace Services.Communication.DataStructures.Queues.Reliable
{
    using System;
    using Chains.Play;

    [Serializable]
    public sealed class UnacknowledgedItem : SerializableSpecification
    {
        public ExecutableActionSpecification Specification { get; set; }

        public DateTime DateIssued { get; set; }

        public int Id { get; set; }

        public override int DataStructureVersionNumber
        {
            get
            {
                return 1;
            }
        }
    }
}
