namespace Chains.Play.Installation
{
    using System.Collections;
    using System.Configuration.Install;
    using System.Reflection;
    using System.ServiceProcess;

    public class InstallAsService : ReproducibleWithData<InstallAsServiceData>,
        IChainableAction<InstallationContext, InstallationContext>
    {
        public InstallAsService(InstallAsServiceData data)
            : base(data)
        {
        }

        public InstallationContext Act(InstallationContext context)
        {
            using (var installer = GetInstaller(Data))
            {
                IDictionary state = new Hashtable();
                try
                {
                    installer.Install(state);
                    installer.Commit(state);
                }
                catch
                {
                    try
                    {
                        installer.Rollback(state);
                    }
                    catch
                    {
                    }

                    throw;
                }
            }

            return context;
        }

        internal static ServiceInstaller GetInstaller(InstallAsServiceData data)
        {
            var serviceInstaller = new ServiceInstaller();
            var serviceProcessInstaller = new ServiceProcessInstaller();
            serviceInstaller.ServiceName = data.Name;
            serviceInstaller.Description = data.Description;
            serviceProcessInstaller.Account = data.Account;
            serviceProcessInstaller.Installers.Add(serviceInstaller);

            serviceInstaller.StartType = ServiceStartMode.Automatic;
            serviceInstaller.DelayedAutoStart = data.DelayAutoStart;
            serviceInstaller.Context = new InstallContext();
            serviceInstaller.Context.Parameters["assemblypath"] = Assembly.GetEntryAssembly().Location;

            return serviceInstaller;
        }
    }
}
