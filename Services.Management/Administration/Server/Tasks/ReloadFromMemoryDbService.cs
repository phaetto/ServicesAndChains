namespace Services.Management.Administration.Server.Tasks
{
    using System.Collections.Generic;
    using System.Net.Sockets;
    using Chains;
    using Chains.Play;
    using Chains.Play.Web;
    using Services.Communication.DataStructures.NameValue;
    using Services.Communication.Protocol;

    internal class ReloadFromMemoryDbService : IChainableAction<Client, Dictionary<string, WorkUnitReportData>>
    {
        public Dictionary<string, WorkUnitReportData> Act(Client context)
        {
            try
            {
                using (var connectionToHashMemoryStorage = context.Do(new OpenConnection()))
                {
                    var serializedReports =
                        connectionToHashMemoryStorage.Do(
                            new Send<KeyValueData>(
                                new GetKeyValue(
                                    new KeyData
                                    {
                                        Key = "report-data"
                                    }))).Value;

                    var previousAdminReportData = Json<Dictionary<string, WorkUnitReportData>>.Deserialize(
                        serializedReports);

                    if (previousAdminReportData.ContainsKey(AdministrationContext.ReloadingMemoryDbServiceName))
                    {
                        previousAdminReportData[AdministrationContext.ReloadingMemoryDbServiceName].AdviceGiven =
                            AdviceState.Stop;
                    }

                    return previousAdminReportData;
                }
            }
            catch (SocketException)
            {
            }

            return null;
        }
    }
}
