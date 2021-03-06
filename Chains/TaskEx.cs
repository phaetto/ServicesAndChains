﻿namespace Chains
{
    using System.Threading;
    using System.Threading.Tasks;

    public static class TaskEx
    {
        public static Task<T> FromResult<T>(T result)
        {
            var taskCompletionSource = new TaskCompletionSource<T>();
            taskCompletionSource.TrySetResult(result);
            return taskCompletionSource.Task;
        }

        public static Task Delay(int milliseconds)
        {
            var taskCompletionSource = new TaskCompletionSource<object>();
            new Timer(_ => taskCompletionSource.SetResult(null)).Change(milliseconds, -1);
            return taskCompletionSource.Task;
        }
    }
}
