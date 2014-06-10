namespace Services.Windows.Initiator
{
    using System;
    using System.ServiceProcess;
    using Chains.Play.Installation;

    static class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                var serviceData = new InstallAsServiceData
                                  {
                                      Account = ServiceAccount.LocalSystem,
                                      DelayAutoStart = false,
                                      Description =
                                          "Initiates the admin host and other processes in the default service window station.",
                                      Name = "Admin Initiator"
                                  };

                if (args[0] == "--install")
                {
                    new InstallationContext().Do(new InstallAsService(serviceData));
                    Console.WriteLine("Service has been installed.");
                }
                else if (args[0] == "--uninstall")
                {
                    new InstallationContext().Do(new UninstallService(serviceData));
                    Console.WriteLine("Service has been uninstalled.");
                }
            }

            if (!Environment.UserInteractive)
            {
                ServiceBase.Run(
                    new ServiceBase[]
                    {
                        new AdminService()
                    });
            }
        }
    }
}
