namespace Chains
{
    public class ChainWithHistoryAndParent<T, TParentType> : ChainWithHistory<T>
        where T : Chain<T>
        where TParentType : AbstractChain
    {
        public readonly TParentType Parent;

        public ChainWithHistoryAndParent(TParentType chain)
        {
            Parent = chain;
        }

        public static implicit operator ChainWithParent<T, TParentType>(ChainWithHistoryAndParent<T, TParentType> chainWithHistoryAndParent)
        {
            return new ChainWithParent<T, TParentType>(chainWithHistoryAndParent.Parent);
        }

        public static implicit operator ChainWithHistoryAndParent<T, TParentType>(ChainWithParent<T, TParentType> chainWithParent)
        {
            return new ChainWithHistoryAndParent<T, TParentType>(chainWithParent.Parent);
        }
    }
}
