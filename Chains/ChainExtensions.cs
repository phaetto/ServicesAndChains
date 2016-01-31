namespace Chains
{
    using System;

    public static class ChainExtensions
    {
        public static TReturnChainType DoIf<T, TReturnChainType>(
            this T context,
            Func<T, bool> predicate,
            IChainableAction<T, TReturnChainType> action)
            where T : Chain<T>
            where TReturnChainType : class
        {
            return predicate(context) ? context.Do(action) : null;
        }
    }
}
