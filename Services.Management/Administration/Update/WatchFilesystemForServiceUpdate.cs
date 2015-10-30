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

    public sealed class WatchFilesystemForServiceUpdate : Chain<WatchFilesystemForServiceUpdate>,
        IDisposable,
        IWorkerEvents
    {
        public readonly int secondsChecking;

        public readonly string file;

        public readonly string executionerMainFile;

        public readonly string serviceName;

        private ClientConnectionContext adminServer;

        private CheckIfThisProcessIsUpdated checkIfThisProcessIsUpdated;

        private readonly WorkUnitContext workUnitContext;

        public WatchFilesystemForServiceUpdate(
            string serviceName, string file, string executionerMainFile, int secondsChecking = 60, WorkUnitContext workUnitContext = null)
        {
            this.file = file;
            this.secondsChecking = secondsChecking;
            this.executionerMainFile = executionerMainFile;
            this.serviceName = serviceName;
            this.workUnitContext = workUnitContext;
            ThreadPool.QueueUserWorkItem(CheckFileSystemThread);
        }

        private void CheckFileSystemThread(object state)
        {
            var lastChangedTime = File.GetLastWriteTimeUtc(file);

            workUnitContext.LogLine($"Service updater started looking at '{file}'");
            workUnitContext.LogLine($"File has last been updated on '{lastChangedTime}'");

            while (true)
            {
                var currentFileTime = File.GetLastWriteTimeUtc(file);

                if (currentFileTime > lastChangedTime)
                {
                    workUnitContext.LogLine("Updating service '" + serviceName + "' with a new version...");
                    try
                    {
                        adminServer.Do(
                            new Send(
                                new ApplyServiceVersionUpdate(
                                    new UpdateWorkUnitData
                                    {
                                        UpdateFolderOrFile = Path.GetDirectoryName(file),
                                        ServiceName = serviceName
                                    })));

                        workUnitContext.LogLine(
                            "Update finished for '" + serviceName + "' at " + DateTime.UtcNow.ToString() + " (utc)");
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
        }

        public void OnStart()
        {
            adminServer =
                new Client(workUnitContext.WorkerData.AdminHost, workUnitContext.WorkerData.AdminPort).Do(
                    new OpenConnection());

            checkIfThisProcessIsUpdated = new CheckIfThisProcessIsUpdated(
                executionerMainFile, secondsChecking, adminServer, workUnitContext);
            checkIfThisProcessIsUpdated.OnStart();
        }

        public void OnStop()
        {
            checkIfThisProcessIsUpdated.OnStop();
        }
    }
}
