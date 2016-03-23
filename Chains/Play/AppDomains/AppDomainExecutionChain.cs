namespace Chains.Play.AppDomains
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using Chains.Exceptions;

    public sealed class AppDomainExecutionChain : Chain<AppDomainExecutionChain>, IDisposable
    {
        private readonly RemoteReplayProxy proxy;

        public AppDomain Domain { get; private set; }

        public AppDomainExecutionChain(string appDomainName, string contextName)
        {
            Check.ArgumentNullOrEmpty(appDomainName, nameof(appDomainName));
            Check.ArgumentNullOrEmpty(contextName, nameof(contextName));

            Domain = AppDomain.CreateDomain(appDomainName, AppDomain.CurrentDomain.Evidence, AppDomain.CurrentDomain.SetupInformation);

            var type = typeof(RemoteReplayProxy);

            proxy = (RemoteReplayProxy)Domain.CreateInstanceAndUnwrap(
                type.Assembly.FullName,
                type.FullName,
                false,
                BindingFlags.Default,
                null,
                new object[] { contextName },
                CultureInfo.InvariantCulture,
                null);
        }

        internal RemoteReplayProxy Proxy => proxy;

        public void Dispose()
        {
            if (Domain != null)
            {
                try
                {
                    AppDomain.Unload(Domain);
                }
                finally
                {
                    Domain = null;
                }
            }
        }
    }
}
