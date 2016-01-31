namespace Chains.Play.Streams
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    public abstract class Publisher : IDisposable
    {
        private readonly List<PublisherSubscription> subscriptions = new List<PublisherSubscription>();

        public IEnumerable<object> AsStream()
        {
            return AsStream(CancellationToken.None);
        }

        public IEnumerable<object> AsStream(CancellationToken cancellationToken)
        {
            var newSubscription = new PublisherSubscription()
                {
                    CancellationToken = cancellationToken,
                    EventStream = new EventStream()
                };

            lock (subscriptions)
            {
                subscriptions.Add(newSubscription);
            }

            return newSubscription.EventStream.AsStream(cancellationToken);
        }


        public IEnumerable<TAction> AsStream<TAction>()
        {
            return AsStream<TAction>(CancellationToken.None);
        }

        public IEnumerable<TAction> AsStream<TAction>(CancellationToken cancellationToken)
        {
            var newSubscription = new PublisherSubscription()
            {
                CancellationToken = cancellationToken,
                EventStream = new EventStream()
            };

            lock (subscriptions)
            {
                subscriptions.Add(newSubscription);
            }

            return newSubscription.EventStream.AsStream<TAction>(cancellationToken);
        }

        public void Publish(object action)
        {
            var subscriptionsCancelled = new List<PublisherSubscription>();

            foreach (var publisherSubscription in subscriptions)
            {
                if (!publisherSubscription.CancellationToken.IsCancellationRequested)
                {
                    publisherSubscription.EventStream.PublishToCollection(action);
                }
                else
                {
                    subscriptionsCancelled.Add(publisherSubscription);
                }
            }

            lock (subscriptions)
            {
                foreach (var subscriptionCancelled in subscriptionsCancelled)
                {
                    subscriptionCancelled.EventStream.Dispose();
                    subscriptions.Remove(subscriptionCancelled);
                }
            }
        }

        public virtual void Dispose()
        {
            lock (subscriptions)
            {
                foreach (var publisherSubscription in subscriptions)
                {
                    publisherSubscription.EventStream.Dispose();
                }
            }
        }
    }
}
