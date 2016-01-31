namespace Chains
{
    public class ChainWithParent<T, TParentType> : Chain<T>
        where T : Chain<T>
        where TParentType : AbstractChain
    {
        public readonly TParentType Parent;

        public ChainWithParent(TParentType chain)
        {
            Parent = chain;
        }
    }
}
