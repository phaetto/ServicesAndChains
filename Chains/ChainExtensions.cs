namespace Chains
{
    using System;

    public static class ChainExtensions
    {
        public static ReturnChainType DoIf<T, ReturnChainType>(
            this T context,
            Func<T, bool> predicate,
            IChainableAction<T, ReturnChainType> action)
            where T : Chain<T>
            where ReturnChainType : class
        {
            return predicate(context) ? context.Do(action) : null;
        }

        public static ReturnChainType DoIfNotNull<T, ReturnChainType>(
            this T context,
            IChainableAction<T, ReturnChainType> action)
            where T : Chain<T>
            where ReturnChainType : class
        {
            return context != null ? context.Do(action) : null;
        }

        public static T IfNullChangeTo<T>(
            this T context,
            T otherObject)
            where T : Chain<T>
        {
            return context ?? otherObject;
        }

        public static T IfTrueRevert<T>(this T context, Func<T, bool> predicate)
            where T : Chain<T>
        {
            return predicate(context) ? Chain<T>.LastKnownGoodObject : context;
        }

        public static T IfNullRevert<T>(this T context)
            where T : Chain<T>
        {
            return context ?? Chain<T>.LastKnownGoodObject;
        }
    }
}
