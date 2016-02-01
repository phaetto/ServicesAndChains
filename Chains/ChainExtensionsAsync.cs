namespace Chains
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public static class ChainExtensionsAsync
    {
        public static Task<TReturnChainType> Do<T, TReturnChainType>(
            this Task<T> context,
            IChainableAction<T, Task<TReturnChainType>> action)
            where T : Chain<T>
        {
            return context.ContinueWith(x => x.Result.Do(action)).Unwrap();
        }

        public static Task<TReturnChainType> Do<T, TReturnChainType>(
            this Task<T> context,
            IChainableAction<T, TReturnChainType> action)
            where T : Chain<T>
        {
            return context.ContinueWith(x => x.Result.Do(action));
        }

        public static Task<IEnumerable<TReturnChainType>> Do<T, TReturnChainType>(
            this Task<T> context,
            IEnumerable<IChainableAction<T, TReturnChainType>> actions)
            where T : Chain<T>
        {
            return context.ContinueWith(x => x.Result.Do(actions));
        }

        public static Task<TReturnChainType> DoAsync<T, TReturnChainType>(
            this Task<T> context,
            IChainableAction<T, TReturnChainType> action)
            where T : Chain<T>
        {
            return context.ContinueWith(x => x.Result.Do(action));
        }
    }
}
