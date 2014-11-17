namespace Services.Management.Administration.Worker
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Chains;

    public sealed class PrepareAssembliesInWorkUnit : IChainableAction<WorkUnitContext, WorkUnitContext>
    {
        public WorkUnitContext Act(WorkUnitContext context)
        {
            if (!string.IsNullOrEmpty(context.WorkerData.DllPath))
            {
                context.WorkerData.DllPath = CalculateAssemblyPath(context.WorkerData.DllPath);

                if (File.Exists(context.WorkerData.DllPath + ".config"))
                {
                    var processConfig = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar
                        + context.WorkerData.HostProcessFileName + ".config";

                    if (File.Exists(processConfig))
                    {
                        File.Delete(processConfig);
                    }

                    File.Copy(context.WorkerData.DllPath + ".config", processConfig);
                }

                LoadAssembly(context.WorkerData.DllPath);
            }

            return context;
        }

        private static string CalculateAssemblyPath(string assemblyPath)
        {
            assemblyPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assemblyPath));
            return assemblyPath;
        }

        private static void LoadAssembly(string assemblyPath)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            if (assemblies.All(x => x.GetName().Name != Path.GetFileNameWithoutExtension(assemblyPath)))
            {
                Assembly.LoadFile(assemblyPath);
            }
        }
    }
}
