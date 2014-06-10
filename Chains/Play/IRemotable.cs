namespace Chains.Play
{
    public interface IRemotable : IReproducible
    {
        ExecutableActionSpecification ReturnData { get; }
    }
}
