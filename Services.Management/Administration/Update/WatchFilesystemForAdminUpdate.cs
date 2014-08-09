namespace Services.Management.Administration.Update
{
    using System;
    using System.IO;
    using System.Net.Sockets;
    using System.Threading;
    using Chains;
    using Chains.Play.Web;
    using Services.Communication.Protocol;
    using Services.Management.Administration.Server;
    using Services.Management.Administration.Worker;

    public sealed class WatchFilesystemForAdminUpdate : Chain<WatchFilesystemForAdminUpdate>,
        IDisposable,
        IWorkerEvents
    {
        public readonly Thread checkThread = null;

        public readonly int secondsChecking;

        public readonly string file;

        private ClientConnectionContext adminServer;

        private readonly WorkUnitContext workUnitContext;

        public WatchFilesystemForAdminUpdate(string file, int secondsChecking = 60, WorkUnitContext workUnitContext = null)
        {
            this.file = file;
            this.secondsChecking = secondsChecking;
            this.workUnitContext = workUnitContext;
            checkThread = new Thread(CheckFileSystemThread);
        }

        private void CheckFileSystemThread()
        {
            var lastChangedTime = File.GetLastWriteTimeUtc(file);

            workUnitContext.LogLine(string.Format("Updater started looking at '{0}'", file));
            workUnitContext.LogLine(string.Format("File has last been updated on '{0}'", lastChangedTime));

            var serverDateTimeStarted = DateTime.MinValue;

            while (true)
            {
                Thread.Sleep(secondsChecking * 1000);

                var currentFileTime = File.GetLastWriteTimeUtc(file);

                if (currentFileTime > lastChangedTime)
                {
                    workUnitContext.LogLine("Found new update! Sending signal...");
                    try
                    {
                        serverDateTimeStarted =
                            adminServer.Do(new Send<AdministrationData>(new GetAdministratorData())).StartedOn;

                        adminServer.Do(
                            new Send(
                                new ApplyAdminUpdate(Path.GetDirectoryName(file))
                                {
                                    ApiKey = workUnitContext.ApiKey,
                                    Session = workUnitContext.Session
                                }));
                        break;
                    }
                    catch (SocketException)
                    {
                        adminServer.Do(new RetryConnection());
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            workUnitContext.LogLine("Waiting for the new admin instance to come up...");

            adminServer.Do(new WaitUntilNewAdminServerIsUp(serverDateTimeStarted))
                       .Do(
                           new Send(
                               new StartWorkerProcessWithDelay(
                                   new WorkerDataWithDelay
                                   {
                                       WorkerData = workUnitContext.WorkerData,
                                       DelayInSeconds = secondsChecking
                                   })
                               {
                                   ApiKey = workUnitContext.ApiKey,
                                   Session = workUnitContext.Session
                               }));

            workUnitContext.Close();
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
        }

        public void OnStart()
        {
            adminServer =
                new Client(workUnitContext.WorkerData.AdminHost, workUnitContext.WorkerData.AdminPort).Do(
                    new OpenConnection());

            checkThread.Start();
        }

        public void OnStop()
        {
            checkThread.Abort();
        }
    }
}
