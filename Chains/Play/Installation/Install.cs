namespace Chains.Play.Installation
{
    using System.Collections;
    using System.Configuration.Install;

    public class Install : ReproducibleWithData<InstallData>,
        IChainableAction<InstallationContext, InstallationContext>
    {
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
            }

            return context;
        }
    }
}
