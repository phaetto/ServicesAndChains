namespace Services.Management.Administration.Server
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using Chains;
    using Chains.Play;
    using Chains.Play.Security;
    using Chains.Play.Web;
    using Services.Communication.DataStructures.NameValue;
    using Services.Communication.Protocol;
    using Services.Management.Administration.Worker;

    public class CloseAdminForUpdate : Reproducible,
        IChainableAction<AdministrationContext, AdministrationContext>,
        IAuthorizableAction, IApplicationAuthorizableAction
    {
        public AdministrationContext Act(AdministrationContext context)
        {
            ThreadPool.QueueUserWorkItem(
                x =>
                {
                    // This should be here for the tcp server to answer back
                    Thread.Sleep(500);

                    // Start a memory-hash to keep the existing logged data
                    context.Do(
                        new StartWorkerProcess(
                            new StartWorkerData
                            {
                                AdminHost = context.AdminServer.Parent.Parent.Hostname,
                                AdminPort = context.AdminServer.Parent.Parent.Port,
                                ContextServerHost = "127.0.0.1",
                                ContextServerPort = 51234,
                                ContextType = typeof(HashContext).FullName,
                                HostProcessFileName = context.HostProcessName,
                                Id = AdministrationContext.ReloadingMemoryDbServiceName,
                                ServiceName = AdministrationContext.GeneralServiceName,
                                ReportUpdateIntervalInSeconds = 1,
                            }));

                    using (var connectionToHashMemoryStorage = new Client("127.0.0.1", 51234).Do(new WaitUntilClientConnects(1000)))
                    {
                        connectionToHashMemoryStorage.Do(
                            new Send(
                                new AddKeyValue(
                                    new KeyValueData
                                    {
                                        Key = "report-data",
                                        Value =
                                            Json<Dictionary<string, WorkUnitReportData>>.Serialize(context.ReportData)
                                    })));
                    }

                    context.Close();
                    Process.GetCurrentProcess().Kill();
                });
            
            return context;
        }

        public string Session { get; set; }

        public string ApiKey { get; set; }
    }
}
