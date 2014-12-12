namespace Services.Management.Administration.Server.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Chains;
    using Services.Management.Administration.Worker;

    internal sealed class CleanUpAdminReports : IChainableAction<AdministrationContext, AdministrationContext>
    {
        public AdministrationContext Act(AdministrationContext context)
        {
            var reportData = context.ReportData;

            var folders = Directory.GetDirectories(context.ServicesFolder, "*");

            foreach (var folder in folders)
            {
                var folderInfo = new DirectoryInfo(folder);
                var versions = Directory.GetDirectories(folder, "*");
                foreach (var version in versions)
                {
                    var versionInfo = new DirectoryInfo(version);

                    int versionAsInt;
                    if (!int.TryParse(versionInfo.Name, out versionAsInt))
                    {
                        continue;
                    }

                    var existingInstanceFolders = Directory.GetDirectories(version, "*");
                    foreach (var existingInstanceFolder in existingInstanceFolders.Select(x => new DirectoryInfo(x)))
                    {
                        lock (reportData)
                        {
                            if (!reportData.ContainsKey(existingInstanceFolder.Name))
                            {
                                reportData.Add(
                                    existingInstanceFolder.Name,
                                    new WorkUnitReportData
                                    {
                                        AdviceGiven = AdviceState.Continue,
                                        WorkerState = WorkUnitState.Abandoned,
                                        Errors = new List<Exception>(),
                                        LastTimeContacted = existingInstanceFolder.CreationTimeUtc,
                                        Log = string.Empty,
                                        StartData = new StartWorkerData
                                                    {
                                                        ServiceName = folderInfo.Name,
                                                        Id = existingInstanceFolder.Name,
                                                        Version = versionAsInt
                                                    },
                                        StartedTime = existingInstanceFolder.CreationTimeUtc,
                                        Uptime = new TimeSpan(0)
                                    });
                            }
                        }
                    }
                }
            }

            foreach (var serviceId in reportData.Keys.ToList())
            {
                var reportWorkerData = reportData[serviceId];

                if (reportWorkerData.WorkerState == WorkUnitState.Running)
                {
                    var timespan = DateTime.UtcNow - reportWorkerData.LastTimeContacted;
                    if (timespan.TotalSeconds > (reportWorkerData.StartData.ReportUpdateIntervalInSeconds + 1))
                    {
                        lock (reportData)
                        {
                            reportData[serviceId].WorkerState = WorkUnitState.Stopping;
                        }
                    }
                    else
                    {
                        reportWorkerData.Uptime = DateTime.UtcNow - reportWorkerData.StartedTime;
                    }
                }
            }

            return context;
        }
    }
}
