namespace Services.Management.Administration.Server
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Chains;
    using Chains.Play;
    using Chains.Play.Security;

    public sealed class GetAdminTypes : RemotableAction<GetFileTypesReturnData, AdministrationContext>,
        IAuthorizableAction,
        IApplicationAuthorizableAction
    {
        public override GetFileTypesReturnData Act(AdministrationContext context)
        {
            return new GetFileTypesReturnData
                   {
                       Types =
                           AppDomain.CurrentDomain.GetAssemblies()
                                    .Where(x => !x.FullName.StartsWith("System") && !x.FullName.StartsWith("Microsoft"))
                                    .SelectMany(assembly => assembly.GetTypes().Where(type =>
                                       type.IsPublic
                                           && !type.IsAbstract
                                           && !type.FullName.StartsWith("System")
                                           && !type.FullName.StartsWith("Microsoft")
                                           && type.IsSubclassOf(typeof(AbstractChain))
                                           && !type.IsGenericType
                                           && !type.IsDefined(typeof(CompilerGeneratedAttribute), false))
                                               .Select(type => type.AssemblyQualifiedName))
                                    .ToArray()
                   };
        }

        public string Session { get; set; }

        public string ApiKey { get; set; }
    }
}
