namespace Chains
{
    using System.Threading;
    using System.Threading.Tasks;

    public static class TaskEx
    {
        static public Task<T> FromResult<T>(T result)
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.TrySetResult(result);
            return tcs.Task;
        }

        static public Task Delay(int milliseconds)
        {
            var tcs = new TaskCompletionSource<object>();
            new Timer(_ => tcs.SetResult(null)).Change(milliseconds, -1);
            return tcs.Task;
        }
    }
}
