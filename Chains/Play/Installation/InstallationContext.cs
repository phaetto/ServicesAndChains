namespace Chains.Play.Installation
{
    using System.Reflection;

    public sealed class InstallationContext : Chain<InstallationContext>
    {
        public readonly Assembly[] InstallableAssemblies;

        public InstallationContext()
        {   
        }

        public InstallationContext(Assembly installableAssembly)
        {
            InstallableAssemblies = new[] { installableAssembly };
        }

        public InstallationContext(Assembly[] installableAssemblies)
        {
            InstallableAssemblies = installableAssemblies;
        }
    }
}
