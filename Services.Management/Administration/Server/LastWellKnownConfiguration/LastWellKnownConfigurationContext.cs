namespace Services.Management.Administration.Server.LastWellKnownConfiguration
{
    using System.Collections.Concurrent;
    using Chains;

    public sealed class LastWellKnownConfigurationContext : ChainWithParent<LastWellKnownConfigurationContext, AdministrationContext>
    {
        public const string ConfigurationFolderName = "LastWellKnownConfiguration";

        public const string AssemblyBlendingFolderName = "AssemblyBlending";

        public const string LastWellKnownConfigurationFilesRepository = "Last Well Known Configuration";

        internal readonly ConcurrentQueue<string> ServicesToCreateSnapshotConcurrentQueue = new ConcurrentQueue<string>();

        internal readonly ConcurrentQueue<ServiceStartedData> ServicesThatHaveStartedConcurrentQueue = new ConcurrentQueue<ServiceStartedData>();

        public LastWellKnownConfigurationContext(AdministrationContext chain)
            : base(chain)
        {
        }
    }
}
