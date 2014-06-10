namespace Chains.Play.Installation
{
    using System.Collections;

    public class UninstallService : ReproducibleWithData<InstallAsServiceData>,
        IChainableAction<InstallationContext, InstallationContext>
    {
        public UninstallService(InstallAsServiceData data)
            : base(data)
        {
        }

        public InstallationContext Act(InstallationContext context)
        {
            using (var installer = InstallAsService.GetInstaller(Data))
            {
                IDictionary state = new Hashtable();
                installer.Uninstall(null);
            }

            return context;
        }
    }
}
