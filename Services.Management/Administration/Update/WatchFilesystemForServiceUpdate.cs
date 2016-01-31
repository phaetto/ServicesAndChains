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
        public readonly int SecondsChecking;

        public readonly string File;

        public readonly string ExecutionerMainFile;

        public readonly string ServiceName;

        private ClientConnectionContext adminServer;

        private CheckIfThisProcessIsUpdated checkIfThisProcessIsUpdated;

        private readonly WorkUnitContext workUnitContext;

        public WatchFilesystemForServiceUpdate(
            string serviceName, string file, string executionerMainFile, int secondsChecking = 60, WorkUnitContext workUnitContext = null)
        {
            this.File = file;
            this.SecondsChecking = secondsChecking;
            this.ExecutionerMainFile = executionerMainFile;
            this.ServiceName = serviceName;
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
                    workUnitContext.LogLine("Updating service '" + ServiceName + "' with a new version...");
                    try
                    {
                        adminServer.Do(
                            new Send(
                                new ApplyServiceVersionUpdate(
                                    new UpdateWorkUnitData
                                    {
                                        UpdateFolderOrFile = Path.GetDirectoryName(File),
                                        ServiceName = ServiceName
                                    })));

                        workUnitContext.LogLine(
                            "Update finished for '" + ServiceName + "' at " + DateTime.UtcNow.ToString() + " (utc)");
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
        }

        public void OnStart()
        {
            adminServer =
                new Client(workUnitContext.WorkerData.AdminHost, workUnitContext.WorkerData.AdminPort).Do(
                    new OpenConnection());

            checkIfThisProcessIsUpdated = new CheckIfThisProcessIsUpdated(
                ExecutionerMainFile, SecondsChecking, adminServer, workUnitContext);
            checkIfThisProcessIsUpdated.OnStart();
        }

        public void OnStop()
        {
            checkIfThisProcessIsUpdated.OnStop();
        }
    }
}
