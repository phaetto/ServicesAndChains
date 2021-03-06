﻿namespace Services.Management.Administration.Server
{
    using System.Diagnostics;
    using Chains.Play;

    public sealed class GetAdministratorData : RemotableAction<AdministrationData, AdministrationContext>
    {
        public override AdministrationData Act(AdministrationContext context)
        {
            context.AdministrationData.ProcessId = Process.GetCurrentProcess().Id;

            return context.AdministrationData;
        }
    }
}
