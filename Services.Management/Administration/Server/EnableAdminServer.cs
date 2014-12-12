namespace Services.Management.Administration.Server
{
    using System;
    using System.Threading;
    using Chains;
    using Chains.Play;
    using Chains.Play.Web;
    using Services.Communication.Protocol;
    using Services.Management.Administration.Server.LastWellKnownConfiguration;
    using Services.Management.Administration.Server.Tasks;

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
            var previousAdminReportData = new Client("127.0.0.1", 51234).Do(new ReloadFromMemoryDbService());

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
                    while (!adminContext.IsClosing)
                    {
                        Thread.Sleep(5000);
                    }
                });

            adminContext.AdminTasksThread.Start();

            adminContext.Do(new BeginAdminLoop(previousAdminReportData == null));

            if (previousAdminReportData != null)
            {
                adminContext.Do(new CleanUpMemoryDbServiceLoop());
            }

            adminContext.LastWellKnownConfigurationContext.Do(new BeginLastWellKnownConfigurationLoop());

            adminContext.Do(new RemoveWellKnownStartedServicesLoop());

            return adminContext;
        }
    }
}
