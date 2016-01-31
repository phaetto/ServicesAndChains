namespace Chains.Play.Streams
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    public sealed class EventStream : IDisposable
    {
        private readonly BlockingCollection<object> blockingColection;

        private bool isDisposed;

        public EventStream(BlockingCollection<object> blockingColection = null)
        {
            this.blockingColection = blockingColection ?? new BlockingCollection<object>(new ConcurrentQueue<object>());
        }

        public IEnumerable<object> AsStream()
        {
            return AsStream(CancellationToken.None);
        }

        public IEnumerable<object> AsStream(CancellationToken cancellationToken)
        {
            return AsStream(cancellationToken, null);
        }


        public IEnumerable<TAction> AsStream<TAction>()
        {
            return AsStream<TAction>(CancellationToken.None);
        }

        public IEnumerable<TAction> AsStream<TAction>(CancellationToken cancellationToken)
        {
            // This is wrong
            return AsStream(cancellationToken, typeof(TAction)).Cast<TAction>();
        }

        public void PublishToCollection(object action)
        {
            if (isDisposed)
            {
                return;
            }

            blockingColection.Add(action);
        }

        public void Dispose()
        {
            if (isDisposed)
            {
                return;
            }

            isDisposed = true;

            blockingColection.CompleteAdding();
            blockingColection.Dispose();
        }

        private IEnumerable<object> AsStream(CancellationToken cancellationToken, Type requestedSpecificType)
        {
            IEnumerable<object> enumeration;

            try
            {
                enumeration = blockingColection.GetConsumingEnumerable(cancellationToken);
            }
            catch (ObjectDisposedException)
            {
                yield break;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                yield break;
            }

            var hasAnyMore = false;
            var iterator = enumeration.GetEnumerator();
            do
            {
                try
                {
                    hasAnyMore = iterator.MoveNext();
                }
                catch (ObjectDisposedException)
                {
                    // From BlockingCollection
                    yield break;
                }
                catch (ArgumentNullException)
                {
                    // From mscorlib
                    yield break;
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    yield break;
                }

                var item = iterator.Current;

                if (requestedSpecificType != null && item.GetType() != requestedSpecificType)
                {
                    continue;
                }

                yield return item;

            } while (hasAnyMore);
        }
    }
}
