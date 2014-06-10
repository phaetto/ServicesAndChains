﻿namespace Services.Management.Administration.Update
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
        public readonly Thread checkThread = null;

        public readonly int secondsChecking;

        public readonly string file;

        public readonly string executionerMainFile;

        public readonly string targetFolder;

        private ClientConnectionContext adminServer;

        private CheckIfThisProcessIsUpdated checkIfThisProcessIsUpdated;

        private readonly WorkUnitContext workUnitContext;

        public WatchFilesystemForFileCopy(
            string targetFolder, string file, string executionerMainFile, int secondsChecking = 60, WorkUnitContext workUnitContext = null)
        {
            this.file = file;
            this.secondsChecking = secondsChecking;
            this.executionerMainFile = executionerMainFile;
            this.targetFolder = targetFolder;
            this.workUnitContext = workUnitContext;
            checkThread = new Thread(CheckFileSystemThread);
        }

        private void CheckFileSystemThread()
        {
            var lastChangedTime = File.GetLastWriteTimeUtc(file);

            workUnitContext.LogLine(string.Format("Service updater started looking at '{0}'", file));
            workUnitContext.LogLine(string.Format("File has last been updated on '{0}'", lastChangedTime));

            while (true)
            {
                var currentFileTime = File.GetLastWriteTimeUtc(file);

                if (currentFileTime > lastChangedTime)
                {
                    workUnitContext.LogLine("Updating folder '" + targetFolder + "' by copying files...");
                    try
                    {
                        adminServer.Do(
                            new Send(
                                new CopyFilesToFolder(
                                    new CopyFilesToFolderData
                                    {
                                        FolderToUpdate = targetFolder,
                                        File = file
                                    })
                                {
                                    ApiKey = workUnitContext.ApiKey,
                                    Session = workUnitContext.Session
                                }));

                        workUnitContext.LogLine(
                            "Update finished for '" + targetFolder + "' at " + DateTime.UtcNow.ToString() + " (utc)");
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

                Thread.Sleep(secondsChecking * 1000);
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

            try
            {
                checkThread.Abort();
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
                    executionerMainFile, secondsChecking, adminServer, workUnitContext);
                checkIfThisProcessIsUpdated.OnStart();
            }

            checkThread.Start();
        }

        public void OnStop()
        {
        }
    }
}
