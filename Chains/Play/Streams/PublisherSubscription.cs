namespace Chains.Play.Streams
{
    using System.Threading;

    public sealed class PublisherSubscription
    {
        public EventStream EventStream { get; set; }

        public CancellationToken CancellationToken { get; set; }
    }
}
