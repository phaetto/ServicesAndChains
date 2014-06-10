namespace Chains
{
    public class ChainWithHistoryAndParent<T, ParentType> : ChainWithHistory<T>
        where T : Chain<T>
        where ParentType : AbstractChain
    {
        public readonly ParentType Parent;

        public ChainWithHistoryAndParent(ParentType chain)
        {
            Parent = chain;
        }

        public static implicit operator ChainWithParent<T, ParentType>(ChainWithHistoryAndParent<T, ParentType> chainWithHistoryAndParent)
        {
            return new ChainWithParent<T, ParentType>(chainWithHistoryAndParent.Parent);
        }

        public static implicit operator ChainWithHistoryAndParent<T, ParentType>(ChainWithParent<T, ParentType> chainWithParent)
        {
            return new ChainWithHistoryAndParent<T, ParentType>(chainWithParent.Parent);
        }
    }
}
