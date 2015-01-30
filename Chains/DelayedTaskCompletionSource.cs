namespace Chains
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class DelayTaskCompletionSource : TaskCompletionSource<bool>, IDisposable
    {
        public Timer Timer { get; set; }

        public void Dispose()
        {
            if (Timer != null)
            {
                Timer.Dispose();
                Timer = null;

                /*
                 * Tasks and timers do not go well together.
                 * Unfortunately I need this here until I find another solution
                 * or remove Task.Delay to stop leaving uncollected garbage around
                 */
                GC.Collect(1, GCCollectionMode.Default);
            }
        }
    }
}
