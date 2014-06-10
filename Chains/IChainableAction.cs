namespace Chains
{
    public interface IChainableAction<in ChainType, out ReturnChainType>
    {
        ReturnChainType Act(ChainType context);
    }
}
