namespace Chains
{
    using System.Threading.Tasks;

    public static class ChainExtensionsAsync
    {
        public static Task<ReturnChainType> Do<T, ReturnChainType>(
            this Task<T> context,
            IChainableAction<T, Task<ReturnChainType>> action)
            where T : Chain<T>
        {
            return context.ContinueWith(x => x.Result.Do(action)).Unwrap();
        }

        public static Task<ReturnChainType> Do<T, ReturnChainType>(
            this Task<T> context,
            IChainableAction<T, ReturnChainType> action)
            where T : Chain<T>
        {
            return context.ContinueWith(x => x.Result.Do(action));
        }

        public static Task<ReturnChainType> DoAsync<T, ReturnChainType>(
            this Task<T> context,
            IChainableAction<T, ReturnChainType> action)
            where T : Chain<T>
        {
            return context.ContinueWith(x => x.Result.Do(action));
        }
    }
}
