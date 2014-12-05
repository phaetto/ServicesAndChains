namespace Chains.Play
{
    public interface IRemotableAction<in TChain, out TReceived> : IChainableAction<TChain, TReceived>, IRemotable
    {
    }
}
