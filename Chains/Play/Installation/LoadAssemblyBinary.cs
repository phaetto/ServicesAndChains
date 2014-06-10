namespace Chains.Play.Installation
{
    using System.IO;
    using System.Reflection;

    public class LoadAssemblyBinary : ReproducibleWithData<AssemblyData>,
        IChainableAction<InstallationContext, InstallationContext>
    {
        public LoadAssemblyBinary(Assembly assembly)
            : base(null)
        {
            Data = new AssemblyData
                   {
                       Bytes = File.ReadAllBytes(assembly.Location)
                   };
        }

        public LoadAssemblyBinary(AssemblyData data)
            : base(data)
        {
        }

        public InstallationContext Act(InstallationContext context)
        {
            Assembly.Load(Data.Bytes);
            return context;
        }
    }
}
