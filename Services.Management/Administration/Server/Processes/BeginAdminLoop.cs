﻿namespace Services.Management.Administration.Server.Processes
{
    using System.Threading.Tasks;
    using Chains;

    public sealed class BeginAdminLoop : IChainableAction<AdministrationContext, Task<AdministrationContext>>
    {
        private readonly bool hasJustStarted;

        public BeginAdminLoop(bool hasJustStarted)
        {
            this.hasJustStarted = hasJustStarted;
        }

        public Task<AdministrationContext> Act(AdministrationContext context)
        {
            if (hasJustStarted)
            {
                context.CleanAdminReports();
                context.Do(new StartServicesExtensionsWellKnownService());
                return TaskEx.Delay(1000).ContinueWith(x => context.Do(new BeginAdminLoop(false))).Unwrap();
            }

            context.CleanAdminReports();

            return TaskEx.Delay(1000).ContinueWith(x => context.Do(this)).Unwrap();
        }
    }
}