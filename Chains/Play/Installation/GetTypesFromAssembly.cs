using System;

namespace Chains.Play.Installation
{
    using System.IO;
    using System.Linq;

    public sealed class GetTypesFromAssembly : RemotableActionWithData<GetTypesFromAssemblyData, GetTypesFromAssemblyReturnData, InstallationContext>
    {
        public GetTypesFromAssembly(GetTypesFromAssemblyData data)
            : base(data)
        {
        }

        protected override GetTypesFromAssemblyReturnData ActRemotely(InstallationContext context)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var types = assemblies.First(x => x.GetName().Name == Path.GetFileNameWithoutExtension(Data.FileName))
                          .GetTypes()
                          .Select(x => x.AssemblyQualifiedName)
                          .ToArray();

            return new GetTypesFromAssemblyReturnData
                   {
                       Types = types
                   };
        }
    }
}
