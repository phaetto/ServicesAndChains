namespace Services.Management.Administration.Server
{
    using System.Collections.Generic;
    using System.IO;
    using Chains.Play;
    using Chains.Play.Security;

    public sealed class GetServiceFiles : RemotableActionWithData<GetServiceFilesData, GetServiceFilesReturnData, AdministrationContext>,
        IAuthorizableAction, IApplicationAuthorizableAction
    {
        public GetServiceFiles(GetServiceFilesData data)
            : base(data)
        {
        }

        public override GetServiceFilesReturnData Act(AdministrationContext context)
        {
            var repositoryPath = context.RepositoryFolder + Data.ServiceName + Path.DirectorySeparatorChar
                + Data.Version + Path.DirectorySeparatorChar;
            var fileList = new List<string>();
            fileList.AddRange(Directory.GetFiles(repositoryPath, "*.exe", SearchOption.TopDirectoryOnly));
            fileList.AddRange(Directory.GetFiles(repositoryPath, "*.dll", SearchOption.TopDirectoryOnly));

            return new GetServiceFilesReturnData
                   {
                       Files = fileList.ToArray()
                   };
        }

        public string Session { get; set; }

        public string ApiKey { get; set; }
    }
}
