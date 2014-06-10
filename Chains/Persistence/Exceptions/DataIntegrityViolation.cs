namespace Chains.Persistence.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class DataIntegrityViolationException : Exception
    {
        public DataIntegrityViolationException()
        {
        }

        public DataIntegrityViolationException(string message)
            : base(message)
        {
        }

        public DataIntegrityViolationException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected DataIntegrityViolationException(
            SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
