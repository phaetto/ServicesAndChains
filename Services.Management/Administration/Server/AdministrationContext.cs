namespace Services.Management.Administration.Server
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net.Sockets;
    using System.Threading;
    using Chains;
    using Chains.Play;
    using Chains.Play.Modules;
    using Chains.Play.Web;
    using Services.Communication.DataStructures.NameValue;
    using Services.Communication.Protocol;
    using Services.Management.Administration.Update;
    using Services.Management.Administration.Worker;

    public sealed class AdministrationContext : ChainWithParent<AdministrationContext, ServerHost>, IDisposable, IModular
    {
        public const string ReloadingMemoryDbServiceName = "reload-hash-db-service";
        public const string GeneralServiceName = "General";

        public readonly Dictionary<string, WorkUnitReportData> ReportData = new Dictionary<string, WorkUnitReportData>();
        public readonly string RepositoryFolder;
        public readonly string ServicesFolder;
        public readonly string HostProcessName;
        public AdministrationData AdministrationData = new AdministrationData();

        public List<AbstractChain> Modules { get; set; }

        public Thread AdminTasksThread { get; set; }

        public ServerConnectionContext AdminServer { get; set; }

        public AdministrationContext(
            string hostname,
            int port,
            string hostProcessName = null)
            : this(new ServerHost(hostname, port), hostProcessName)
        {
        }

        public AdministrationContext(
            ServerHost serverHost,
            string hostProcessName = null,
            Dictionary<string, WorkUnitReportData> previousReportData = null)
            : base(serverHost)
        {
            this.Modules = new List<AbstractChain>();
            this.ReportData = previousReportData ?? ReportData;
            this.AdministrationData.StartedOn = DateTime.UtcNow;

            if (string.IsNullOrWhiteSpace(this.RepositoryFolder))
            {
                this.RepositoryFolder = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar
                                        + RepositoryFolderName + Path.DirectorySeparatorChar;
            }

            if (string.IsNullOrWhiteSpace(this.ServicesFolder))
            {
                this.ServicesFolder = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + ServiceFolderName
                                      + Path.DirectorySeparatorChar;
            }

            if (!Directory.Exists(this.RepositoryFolder))
            {
                Directory.CreateDirectory(this.RepositoryFolder);
            }

            if (!Directory.Exists(this.ServicesFolder))
            {
                Directory.CreateDirectory(this.ServicesFolder);
            }

            if (string.IsNullOrEmpty(hostProcessName))
            {
                hostProcessName = Process.GetCurrentProcess().ProcessName + ".exe";
            }

            HostProcessName = hostProcessName;
        }

        public void Log(string message)
        {
            lock (AdministrationData)
            {
                LogMessageFormatter(message);
            }
        }

        public void Log(string message, params object[] arguments)
        {
            lock (AdministrationData)
            {
                LogMessageFormatter(string.Format(message, arguments));
            }
        }

        public void LogException(Exception exception)
        {
            lock (AdministrationData)
            {
                LogMessageFormatter(
                    string.Format(
                        "{0}\n\n{1}\n\n{2}\n\n", exception.Message, exception.GetType().FullName, exception.StackTrace));
            }
        }

        public void LogLine(string message)
        {
            lock (AdministrationData)
            {
                LogMessageFormatter(message + Environment.NewLine);
            }
        }

        public void LogLine(string message, params object[] arguments)
        {
            lock (AdministrationData)
            {
                LogMessageFormatter(string.Format(message, arguments) + Environment.NewLine);
            }
        }

        public void Close()
        {
            if (AdminServer != null)
                AdminServer.Close();

            AdminTasksThread.Abort();
        }

        public void CleanAdminReports()
        {
            var folders = Directory.GetDirectories(ServicesFolder, "*");

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

                    var deadInstances = Directory.GetDirectories(version, "*");
                    foreach (var deadInstanceInfo in deadInstances.Select(x => new DirectoryInfo(x)))
                    {
                        lock (ReportData)
                        {
                            if (!ReportData.ContainsKey(deadInstanceInfo.Name))
                            {
                                ReportData.Add(
                                    deadInstanceInfo.Name,
                                    new WorkUnitReportData
                                    {
                                        AdviceGiven = AdviceState.Continue,
                                        WorkerState = WorkUnitState.Abandoned,
                                        Errors = new List<Exception>(),
                                        LastTimeContacted = deadInstanceInfo.CreationTimeUtc,
                                        Log = string.Empty,
                                        StartData = new StartWorkerData
                                                    {
                                                        ServiceName = folderInfo.Name,
                                                        Id = deadInstanceInfo.Name,
                                                        Version = versionAsInt
                                                    },
                                        StartedTime = deadInstanceInfo.CreationTimeUtc,
                                        Uptime = new TimeSpan(0)
                                    });
                            }
                        }
                    }
                }
            }

            foreach (var serviceId in ReportData.Keys.ToList())
            {
                var reportData = ReportData[serviceId];

                if (reportData.WorkerState == WorkUnitState.Running)
                {
                    var timespan = DateTime.UtcNow - reportData.LastTimeContacted;
                    if (timespan.TotalSeconds > (reportData.StartData.ReportUpdateIntervalInSeconds + 1))
                    {
                        lock (ReportData)
                        {
                            ReportData[serviceId].WorkerState = WorkUnitState.Stopping;
                        }
                    }
                    else
                    {
                        reportData.Uptime = DateTime.UtcNow - reportData.StartedTime;
                    }
                }
            }
        }

        internal static Dictionary<string, WorkUnitReportData> ReloadFromMemoryDbService()
        {
            Dictionary<string, WorkUnitReportData> previousAdminReportData = null;

            try
            {
                using (var connectionToHashMemoryStorage = new Client("127.0.0.1", 51234).Do(new OpenConnection()))
                {
                    var serializedReports =
                        connectionToHashMemoryStorage.Do(
                            new Send<KeyValueData>(
                                new GetKeyValue(
                                    new KeyData
                                    {
                                        Key = "report-data"
                                    }))).Value;

                    previousAdminReportData = Json<Dictionary<string, WorkUnitReportData>>.Deserialize(
                        serializedReports);

                    if (previousAdminReportData.ContainsKey(ReloadingMemoryDbServiceName))
                    {
                        previousAdminReportData[ReloadingMemoryDbServiceName].AdviceGiven =
                            AdviceState.Stop;
                    }
                }
            }
            catch (SocketException)
            {
            }

            return previousAdminReportData;
        }

        public void CleanUpMemoryDbService()
        {
            ThreadPool.QueueUserWorkItem(CleanUpMemoryDbServiceCallback);
        }

        private void CleanUpMemoryDbServiceCallback(object o)
        {
            while (true)
            {
                try
                {
                    using (new Client("127.0.0.1", 51234).Do(new OpenConnection()))
                    {
                        Do(new StopWorkerProcess(ReloadingMemoryDbServiceName));
                        Thread.Sleep(1000);
                    }
                }
                catch (SocketException)
                {
                    // Server is down
                    break;
                }
            }

            // Wait until the process is considered stopped
            while (true)
            {
                if (!ReportData.ContainsKey(ReloadingMemoryDbServiceName)
                    || ReportData[ReloadingMemoryDbServiceName].WorkerState != WorkUnitState.Running)
                {
                    break;
                }

                Thread.Sleep(1000);
            }

            Do(new DeleteWorkerProcessEntry(ReloadingMemoryDbServiceName));
        }

        public void Dispose()
        {
            try
            {
                Close();
            }
            catch
            {
            }

            try
            {
                AdminServer.Dispose();
            }
            catch
            {
            }
        }

        private void LogMessageFormatter(string text)
        {
            var formattedText = string.Format(
                "{0} (utc) {1}",
                DateTime.UtcNow.ToString("yyyy-MM-dd hh:mm:ss"),
                text);

            Console.WriteLine(formattedText);

            AdministrationData.ServerLog = formattedText + AdministrationData.ServerLog;
        }
    }
}
