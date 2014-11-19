﻿namespace Services.Management.Administration.Update
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

    public sealed class CheckIfThisProcessIsUpdated : Chain<CheckIfThisProcessIsUpdated>,
        IDisposable,
        IWorkerEvents
    {
        public readonly int secondsChecking;

        public readonly string executionerMainFile;

        private ClientConnectionContext adminServer;

        private readonly WorkUnitContext workUnitContext;

        public CheckIfThisProcessIsUpdated(
            string executionerMainFile, int secondsChecking = 60, ClientConnectionContext clientConnectionContext = null, WorkUnitContext workUnitContext = null)
        {
            adminServer = clientConnectionContext;
            this.secondsChecking = secondsChecking;
            this.executionerMainFile = executionerMainFile;
            this.workUnitContext = workUnitContext;
            ThreadPool.QueueUserWorkItem(CheckForThisServiceUpdateThread);
        }

        private void CheckForThisServiceUpdateThread(object state)
        {
            var lastChangedTime = File.GetLastWriteTimeUtc(executionerMainFile);
            workUnitContext.LogLine("Listening for new updates at '" + executionerMainFile + "'...");

            while (true)
            {
                var currentFileTime = File.GetLastWriteTimeUtc(executionerMainFile);

                if (currentFileTime > lastChangedTime)
                {
                    workUnitContext.LogLine("Found new service update. Sending signal and closing...");
                    try
                    {
                        adminServer.Do(
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

                        workUnitContext.Stop();
                        break;
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
                new Client(workUnitContext.WorkerData.AdminHost, workUnitContext.WorkerData.AdminPort)
                    .Do(new OpenConnection());
        }

        public void OnStop()
        {
        }
    }
}
