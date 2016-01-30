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
        public readonly Thread CheckThread = null;

        public readonly int SecondsChecking;

        public readonly string File;

        private ClientConnectionContext adminServer;

        private readonly WorkUnitContext workUnitContext;

        public WatchFilesystemForAdminUpdate(string file, int secondsChecking = 60, WorkUnitContext workUnitContext = null)
        {
            this.File = file;
            this.SecondsChecking = secondsChecking;
            this.workUnitContext = workUnitContext;
            CheckThread = new Thread(CheckFileSystemThread);
        }

        private void CheckFileSystemThread()
        {
            var lastChangedTime = System.IO.File.GetLastWriteTimeUtc(File);

            workUnitContext.LogLine($"Updater started looking at '{File}'");
            workUnitContext.LogLine($"File has last been updated on '{lastChangedTime}'");

            var serverDateTimeStarted = DateTime.MinValue;

            while (true)
            {
                Thread.Sleep(SecondsChecking * 1000);

                var currentFileTime = System.IO.File.GetLastWriteTimeUtc(File);

                if (currentFileTime > lastChangedTime)
                {
                    workUnitContext.LogLine("Found new update! Sending signal...");
                    try
                    {
                        serverDateTimeStarted =
                            adminServer.Do(new Send<AdministrationData>(new GetAdministratorData())).StartedOn;

                        adminServer.Do(
                            new Send(
                                new ApplyAdminUpdate(Path.GetDirectoryName(File))
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
                                       DelayInSeconds = SecondsChecking
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
                CheckThread.Abort();
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

            CheckThread.Start();
        }

        public void OnStop()
        {
            CheckThread.Abort();
        }
    }
}
