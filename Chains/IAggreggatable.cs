namespace Chains
{
    public interface IAggreggatable<in T>
    {
        void AggregateToThis(T context);
    }
}
