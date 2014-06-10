namespace Chains
{
    using System;
    using Chains.Play;

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

        public static TReceive DoRemoteIf<T, TReceive>(
            this T context, Func<T, bool> predicate,
            RemotableAction<TReceive, T> action)
            where T : Chain<T>
            where TReceive : SerializableSpecification, new()
        {
            return predicate(context) ? context.DoRemotable(action) : null;
        }

        public static TReceive DoRemoteIfNotNull<T, TReceive>(
            this T context,
            RemotableAction<TReceive, T> action)
            where T : Chain<T>
            where TReceive : SerializableSpecification, new()
        {
            return context != null ? context.DoRemotable(action) : null;
        }

        public static TReceive DoRemoteIf<T, TReceive, TSend>(
            this T context, Func<T, bool> predicate,
            RemotableActionWithData<TSend, TReceive, T> action)
            where T : Chain<T>
            where TReceive : SerializableSpecification, new()
            where TSend : SerializableSpecification, new()
        {
            return predicate(context) ? context.DoRemotable(action) : null;
        }

        public static TReceive DoRemoteIfNotNull<T, TReceive, TSend>(
            this T context,
            RemotableActionWithData<TSend, TReceive, T> action)
            where T : Chain<T>
            where TReceive : SerializableSpecification, new()
            where TSend : SerializableSpecification, new()
        {
            return context != null ? context.DoRemotable(action) : null;
        }

        public static TReceive DoRemoteIf<T, TReceive, TSend>(
            this T context, Func<T, bool> predicate,
            RemotableActionWithSerializableData<TSend, TReceive, T> action)
            where T : Chain<T>
            where TReceive : SerializableSpecification, new()
            where TSend : SerializableSpecification, new()
        {
            return predicate(context) ? context.DoRemotable(action) : null;
        }

        public static TReceive DoRemoteIfNotNull<T, TReceive, TSend>(
            this T context,
            RemotableActionWithSerializableData<TSend, TReceive, T> action)
            where T : Chain<T>
            where TReceive : SerializableSpecification, new()
            where TSend : SerializableSpecification, new()
        {
            return context != null ? context.DoRemotable(action) : null;
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
