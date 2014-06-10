namespace Chains.Play
{
    public abstract class ReproducibleWithData<DataType> : ReproducibleWithSerializableData<DataType>
        where DataType : SerializableSpecification, new()
    {
        protected ReproducibleWithData(DataType data) : base(data)
        {
        }
    }
}
