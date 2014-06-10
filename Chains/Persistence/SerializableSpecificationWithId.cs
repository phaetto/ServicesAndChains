namespace Chains.Persistence
{
    using System;
    using Chains.Play;

    public abstract class SerializableSpecificationWithId : SerializableSpecification
    {
        public Guid Id { get; set; }

        public DateTime Created { get; set; }

        protected SerializableSpecificationWithId()
        {
            Created = DateTime.Now;
        }
    }
}
