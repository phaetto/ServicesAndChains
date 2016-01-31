namespace Services.Management.Administration.Update
{
    using System;
    using System.IO;
    using System.Net.Sockets;
    using System.Threading;
    using Chains;
    using Chains.Play.Web;
    using Services.Communication.Protocol;
    using Services.Management.Administration.Worker;

    public sealed class WatchFilesystemForFileCopy : Chain<WatchFilesystemForFileCopy>,
        IDisposable,
        IWorkerEvents
    {
        public readonly int SecondsChecking;

        public readonly string File;

        public readonly string ExecutionerMainFile;

        public readonly string TargetFolder;

        private ClientConnectionContext adminServer;

        private CheckIfThisProcessIsUpdated checkIfThisProcessIsUpdated;

        private readonly WorkUnitContext workUnitContext;

        public WatchFilesystemForFileCopy(
            string targetFolder, string file, string executionerMainFile, int secondsChecking = 60, WorkUnitContext workUnitContext = null)
        {
            this.File = file;
            this.SecondsChecking = secondsChecking;
            this.ExecutionerMainFile = executionerMainFile;
            this.TargetFolder = targetFolder;
            this.workUnitContext = workUnitContext;
            ThreadPool.QueueUserWorkItem(CheckFileSystemThread);
        }

        private void CheckFileSystemThread(object state)
        {
            var lastChangedTime = System.IO.File.GetLastWriteTimeUtc(File);

            workUnitContext.LogLine($"Service updater started looking at '{File}'");
            workUnitContext.LogLine($"File has last been updated on '{lastChangedTime}'");

            while (true)
            {
                var currentFileTime = System.IO.File.GetLastWriteTimeUtc(File);

                if (currentFileTime > lastChangedTime)
                {
                    workUnitContext.LogLine("Updating folder '" + TargetFolder + "' by copying files...");
                    try
                    {
                        adminServer.Do(
                            new Send(
                                new CopyFilesToFolder(
                                    new CopyFilesToFolderData
                                    {
                                        FolderToUpdate = TargetFolder,
                                        File = File
                                    })
                                {
                                    ApiKey = workUnitContext.ApiKey,
                                    Session = workUnitContext.Session
                                }));

                        workUnitContext.LogLine(
                            "Update finished for '" + TargetFolder + "' at " + DateTime.UtcNow.ToString() + " (utc)");
                        lastChangedTime = currentFileTime;
                    }
                    catch (SocketException)
                    {
                        adminServer.Do(new RetryConnection());
                    }
                    catch (Exception ex)
                    {
                        workUnitContext.LogException(ex);
                    }
                }

                Thread.Sleep(SecondsChecking * 1000);
            }
        }

        public void Dispose()
        {
            try
            {
                adminServer.Close();
            }
            catch
            {
            }

            if (checkIfThisProcessIsUpdated != null)
            {
                checkIfThisProcessIsUpdated.Dispose();
            }
        }

        public void OnStart()
        {
            if (adminServer == null)
            {
                adminServer =
                    new Client(workUnitContext.WorkerData.AdminHost, workUnitContext.WorkerData.AdminPort)
                        .Do(new OpenConnection());

                checkIfThisProcessIsUpdated = new CheckIfThisProcessIsUpdated(
                    ExecutionerMainFile, SecondsChecking, adminServer, workUnitContext);
                checkIfThisProcessIsUpdated.OnStart();
            }
        }

        public void OnStop()
        {
        }
    }
}
