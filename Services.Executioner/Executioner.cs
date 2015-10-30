namespace Services.Executioner
{
    using System;
    using System.Diagnostics;
    using System.Windows.Forms;
    using Chains.Play;
    using Services.Management.Administration.Executioner;
    using Services.Management.Administration.Worker;

    class Executioner
    {
        static WorkerExecutioner workerExecutioner;

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine();
                Console.WriteLine("Worker Execution v1.0");
                Console.WriteLine($"Alexander Mantzoukas (c) 2013-{DateTime.Now.Year}");
                Console.WriteLine();

                if (args.Length != 2)
                {
                    var msg = "Wrong number of inputs given." + Environment.NewLine + "You can use:"
                              + Environment.NewLine + "(--execute|--start|--update|--admin|--prepare) {json}"
                              + Environment.NewLine + "<admin host> <admin port>";

                    Console.WriteLine();
                    Console.WriteLine(msg);
                    Console.WriteLine();

                    if (Environment.UserInteractive)
                    {
                        MessageBox.Show(msg, "Wrong arguments");
                    }

                    return;
                }
                
                switch (args[0])
                {
                    case "--execute":
                        {
                            var sessionAndApiKey = DeserializableSpecification<SessionAndApiKey>.DeserializeFromJson(args[1]);
                            var workerData = GetWorkerData(args[1]);
                            workerExecutioner = new WorkerExecutioner(ExecutionMode.Worker, workerData, sessionAndApiKey.Session, sessionAndApiKey.ApiKey);
                            workerExecutioner.Execute();

                            break;
                        }
                    case "--prepare":
                        {
                            var sessionAndApiKey = DeserializableSpecification<SessionAndApiKey>.DeserializeFromJson(args[1]);
                            var workerData = GetWorkerData(args[1]);
                            workerExecutioner = new WorkerExecutioner(ExecutionMode.PrepareWorker, workerData, sessionAndApiKey.Session, sessionAndApiKey.ApiKey);
                            workerExecutioner.Execute();

                            break;
                        }
                    case "--start":
                        {
                            var sessionAndApiKey = DeserializableSpecification<SessionAndApiKey>.DeserializeFromJson(args[1]);
                            var workerData = GetWorkerData(args[1]);
                            workerExecutioner = new WorkerExecutioner(ExecutionMode.StartWorkerFromAdmin, workerData, sessionAndApiKey.Session, sessionAndApiKey.ApiKey);
                            workerExecutioner.Execute();
                            break;
                        }
                    case "--update":
                        {
                            var sessionAndApiKey = DeserializableSpecification<SessionAndApiKey>.DeserializeFromJson(args[1]);
                            var workerData = GetWorkerData(args[1]);
                            workerExecutioner = new WorkerExecutioner(ExecutionMode.UpdateAdministrationServer, workerData, sessionAndApiKey.Session, sessionAndApiKey.ApiKey);
                            workerExecutioner.Execute();
                            break;
                        }
                    case "--admin":
                        {
                            var sessionAndApiKey = DeserializableSpecification<SessionAndApiKey>.DeserializeFromJson(args[1]);
                            var workerData = GetWorkerData(args[1]);
                            workerExecutioner = new WorkerExecutioner(ExecutionMode.AdministrationServer, workerData, sessionAndApiKey.Session, sessionAndApiKey.ApiKey);
                            workerExecutioner.Execute();
                            break;
                        }
                    default:
                        workerExecutioner = new WorkerExecutioner(
                            ExecutionMode.AdministrationServer,
                            new StartWorkerData
                            {
                                AdminHost = args[0],
                                AdminPort = int.Parse(args[1]),
                                HostProcessFileName = "Services.Executioner.exe"
                            });
                        workerExecutioner.Execute();
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine();

                try
                {
                    MessageBox.Show(ex.StackTrace, ex.Message);
                }
                catch
                {
                }

                try
                {
                    workerExecutioner.Dispose();
                }
                catch
                {
                }

                Process.GetCurrentProcess().Kill();
            }
        }

        private static StartWorkerData GetWorkerData(string json)
        {
            var workerData = Json<StartWorkerData>.Deserialize(json);
            if (string.IsNullOrWhiteSpace(workerData.HostProcessFileName))
            {
                workerData.HostProcessFileName = "Services.Executioner.exe";
            }

            return workerData;
        }
    }
}
