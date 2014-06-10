namespace Chains.Play.Installation
{
    using System.Collections;
    using System.Configuration.Install;

    public class Install : ReproducibleWithData<InstallData>,
        IChainableAction<InstallationContext, InstallationContext>
    {
        public Install(bool uninstall = false)
            : base(new InstallData())
        {
            Data = new InstallData { Uninstall = uninstall };
        }

        public Install(InstallData data)
            : base(data)
        {
        }

        public InstallationContext Act(InstallationContext context)
        {
            foreach (var assembly in context.InstallableAssemblies)
            {
                using (var installer = new AssemblyInstaller(assembly, Data.Args))
                {
                    IDictionary state = new Hashtable();
                    installer.UseNewContext = true;
                    try
                    {
                        if (Data.Uninstall)
                        {
                            installer.Uninstall(state);
                        }
                        else
                        {
                            installer.Install(state);
                            installer.Commit(state);
                        }
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
            }

            return context;
        }
    }
}
