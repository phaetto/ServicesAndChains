namespace Chains
{
    public interface IChainableAction<in TChainType, out TReturnChainType>
    {
        TReturnChainType Act(TChainType context);
    }
}
