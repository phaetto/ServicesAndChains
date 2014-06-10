namespace Chains
{
    public class ChainWithParent<T, ParentType> : Chain<T>
        where T : Chain<T>
        where ParentType : AbstractChain
    {
        public readonly ParentType Parent;

        public ChainWithParent(ParentType chain)
        {
            Parent = chain;
        }
    }
}
