﻿namespace Services.Management.Administration.Server
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using Chains;
    using Chains.Play.Modules;
    using Chains.Play.Streams.Timer;
    using Chains.Play.Web;
    using Services.Communication.Protocol;
    using Services.Management.Administration.Server.LastWellKnownConfiguration;

    public sealed class AdministrationContext : ChainWithParent<AdministrationContext, ServerHost>, IDisposable, IModular
    {
        public const string ReloadingMemoryDbServiceName = "reload-hash-db-service";
        public const string GeneralServiceName = "General";
        public const string RepositoryFolderName = "repository";
        public const string ServiceFolderName = "services";
        public const string DataFolderName = "data";
        public const string DefaultFilesRepository = "Admin Repository";

        public readonly Dictionary<string, WorkUnitReportData> ReportData = new Dictionary<string, WorkUnitReportData>();
        public readonly string RepositoryFolder;
        public readonly string ServicesFolder;
        public readonly string DataFolder;
        public readonly string HostProcessName;

        public readonly TimerStreamScheduler TimerStreamScheduler = new TimerStreamScheduler();

        public AdministrationData AdministrationData = new AdministrationData();

        public List<AbstractChain> Modules { get; set; }

        public Thread AdminTasksThread { get; set; }

        public ServerConnectionContext AdminServer { get; set; }

        public CancellationTokenSource AdminCancellationTokenSource => adminCancellationTokenSource;

        internal readonly LastWellKnownConfigurationContext LastWellKnownConfigurationContext;

        private readonly CancellationTokenSource adminCancellationTokenSource = new CancellationTokenSource();

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
            LastWellKnownConfigurationContext = new LastWellKnownConfigurationContext(this);
            Modules = new List<AbstractChain>();
            Modules.Add(LastWellKnownConfigurationContext);

            ReportData = previousReportData ?? ReportData;
            AdministrationData.StartedOn = DateTime.UtcNow;

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

            this.DataFolder =
                Path.GetFullPath(
                    Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + ".."
                    + Path.DirectorySeparatorChar + DataFolderName) + Path.DirectorySeparatorChar;

            if (!Directory.Exists(this.RepositoryFolder))
            {
                Directory.CreateDirectory(this.RepositoryFolder);
            }

            if (!Directory.Exists(this.ServicesFolder))
            {
                Directory.CreateDirectory(this.ServicesFolder);
            }

            if (!Directory.Exists(this.DataFolder))
            {
                Directory.CreateDirectory(this.DataFolder);
            }

            this.AdministrationData.RepositoryFolder = this.RepositoryFolder;
            this.AdministrationData.ServicesFolder = this.ServicesFolder;
            this.AdministrationData.DataFolder = this.DataFolder;

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
                    $"{exception.Message}\n\n{exception.GetType().FullName}\n\n{exception.StackTrace}\n\n");
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
            adminCancellationTokenSource.Cancel();

            if (AdminServer != null)
                AdminServer.Close();
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

            TimerStreamScheduler.Dispose();
        }

        private void LogMessageFormatter(string text)
        {
            var formattedText = $"{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")} (utc) {text}";

            Console.WriteLine(formattedText);

            AdministrationData.ServerLog = formattedText + AdministrationData.ServerLog;
        }
    }
}
