namespace Chains.Play
{
    public abstract class ReproducibleWithData<TDataType> : ReproducibleWithSerializableData<TDataType>
        where TDataType : SerializableSpecification, new()
    {
        protected ReproducibleWithData(TDataType data) : base(data)
        {
        }
    }
}
