namespace Chains.Play
{
    public static class SerializableSpecificationExtensions
    {
        public static byte[] SerializeWithBinaryFormatter(this SerializableSpecification[] specs)
        {
            return SerializableSpecification.SerializeManyWithBinaryFormatter(specs);
        }

        public static string SerializeToBase64String(this SerializableSpecification[] specs)
        {
            return SerializableSpecification.SerializeManyToBase64String(specs);
        }

        public static string SerializeToJson(this SerializableSpecification[] specs)
        {
            return SerializableSpecification.SerializeManyToJson(specs);
        }
    }
}
