using System;

namespace Chains.Play.Installation
{
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;

    internal sealed class GetSpecificTypesFromAssembly : RemotableActionWithData<GetTypesFromAssemblyData, GetTypesFromAssemblyReturnData, InstallationContext>
    {
        public GetSpecificTypesFromAssembly(GetTypesFromAssemblyData data)
            : base(data)
        {
        }

        public override GetTypesFromAssemblyReturnData Act(InstallationContext context)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var types = assemblies.First(x => x.GetName().Name == Path.GetFileNameWithoutExtension(Data.FileName))
                          .GetTypes()
                          .Where(x => !x.IsGenericType && !x.IsAbstract && !x.IsDefined(typeof(CompilerGeneratedAttribute), false))
                          .Select(x => x.AssemblyQualifiedName)
                          .ToArray();

            return new GetTypesFromAssemblyReturnData
                   {
                       Types = types
                   };
        }
    }
}
