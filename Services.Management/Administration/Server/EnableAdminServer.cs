namespace Services.Management.Administration.Server
{
    using System;
    using System.Threading;
    using Chains;
    using Chains.Play;
    using Chains.Play.Web;
    using Services.Communication.Protocol;
    using Services.Management.Administration.Server.LastWellKnownConfiguration;
    using Services.Management.Administration.Server.Processes;

    public sealed class EnableAdminServer : IChainableAction<ServerHost, AdministrationContext>
    {
        public readonly Func<ExecutableActionSpecification[], bool> onBeforeExecute;
        public readonly Action<dynamic> onAfterExecute;
        public readonly string HostProcessName;

        public EnableAdminServer(Func<ExecutableActionSpecification[], bool> onBeforeExecute = null, Action<dynamic> onAfterExecute = null, string hostProcessName = null)
        {
            this.onBeforeExecute = onBeforeExecute;
            this.onAfterExecute = onAfterExecute;
            HostProcessName = hostProcessName;
        }

        public AdministrationContext Act(ServerHost context)
        {
            var previousAdminReportData = AdministrationContext.ReloadFromMemoryDbService();

            var adminContext = new AdministrationContext(
                context, hostProcessName: HostProcessName, previousReportData: previousAdminReportData);

            var name = "AdministrationServer:" + context.Parent.Hostname + ":" + context.Parent.Port;

            adminContext.AdminServer =
                        new ServerHost(context.Parent.Hostname, context.Parent.Port).Do(
                            new StartListen(adminContext,
                                onBeforeExecute: onBeforeExecute,
                                onAfterExecute: onAfterExecute));

            adminContext.AdminTasksThread = new Thread(
                () =>
                {   
                    while (true)
                    {
                        Thread.Sleep(60000);
                    }
                });

            adminContext.AdminTasksThread.Start();

            adminContext.Do(new BeginAdminLoop(previousAdminReportData == null));

            adminContext.CleanUpMemoryDbService();

            adminContext.LastWellKnownConfigurationContext.Do(new BeginLastWellKnownConfigurationLoop());

            return adminContext;
        }
    }
}
