namespace Services.Management.Administration.Server.LastWellKnownConfiguration
{
    using System.Collections.Concurrent;
    using Chains;

    public sealed class LastWellKnownConfigurationContext : ChainWithParent<LastWellKnownConfigurationContext, AdministrationContext>
    {
        internal readonly ConcurrentQueue<string> ServicesToCreateSnapshotConcurrentQueue = new ConcurrentQueue<string>();

        public LastWellKnownConfigurationContext(AdministrationContext chain)
            : base(chain)
        {
        }
    }
}
