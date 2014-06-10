namespace Services.Windows.Initiator
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.ServiceProcess;
    using System.Threading;

    public partial class AdminService : ServiceBase
    {
        public AdminService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Process.Start(
                    new ProcessStartInfo
                    {
                        FileName = ConfigurationManager.AppSettings.Get("script-file"),
                        WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                        UseShellExecute = true,
                        CreateNoWindow = true
                    });

            ThreadPool.QueueUserWorkItem(
                x =>
                {
                    Thread.Sleep(3000);
                    this.Stop();
                });
        }

        protected override void OnStop()
        {
        }
    }
}
