namespace Chains.Play.Installation
{
    using System.Reflection;

    public class LoadAssembly : ReproducibleWithSerializableData<string>,
        IChainableAction<InstallationContext, InstallationContext>
    {
        public LoadAssembly(string file)
            : base(file)
        {
        }

        public InstallationContext Act(InstallationContext context)
        {
            Assembly.LoadFrom(Data);
            return context;
        }
    }
}
