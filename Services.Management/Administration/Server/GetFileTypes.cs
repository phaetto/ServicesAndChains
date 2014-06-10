namespace Services.Management.Administration.Server
{
    using System.IO;
    using System.Reflection;
    using Chains.Play;
    using Chains.Play.AppDomains;
    using Chains.Play.Installation;
    using Chains.Play.Security;

    public sealed class GetFileTypes : RemotableActionWithData<GetFileTypesData, GetFileTypesReturnData, AdministrationContext>,
        IAuthorizableAction, IApplicationAuthorizableAction
    {
        public GetFileTypes(GetFileTypesData data)
            : base(data)
        {
        }

        protected override GetFileTypesReturnData ActRemotely(AdministrationContext context)
        {
            var assemblyPath = context.RepositoryFolder + Data.ServiceName + Path.DirectorySeparatorChar
                + Data.Version + Path.DirectorySeparatorChar + Data.File;

            // Need to do that in a new app domain
            using (var appDomainChain = new AppDomainExecutionChain("load-library", typeof(InstallationContext).AssemblyQualifiedName))
            {
                var fileTypesData =
                    appDomainChain.Do(new ExecuteOverAppDomain(new LoadAssembly(Assembly.GetExecutingAssembly().Location)))
                                  .Do(new ExecuteOverAppDomain(new LoadAssembly(assemblyPath)))
                                  .Do(new ExecuteOverAppDomain<GetTypesFromAssemblyReturnData>(
                                          new GetSpecificTypesFromAssembly(
                                              new GetTypesFromAssemblyData
                                              {
                                                  FileName = Data.File
                                              })));

               return new GetFileTypesReturnData { Types = fileTypesData.Types };
            }
        }

        public string Session { get; set; }

        public string ApiKey { get; set; }
    }
}
