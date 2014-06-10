namespace Services.Management.Administration.Server
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Chains.Play;
    using Chains.Play.Security;

    public sealed class GetAllRepoServices : RemotableAction<GetAllRepoServicesReturnData, AdministrationContext>,
        IAuthorizableAction,
        IApplicationAuthorizableAction
    {
        protected override GetAllRepoServicesReturnData ActRemotely(AdministrationContext context)
        {
            if (string.IsNullOrEmpty(context.RepositoryFolder))
            {
                throw new InvalidOperationException("The repository for the process files cannot be empty");
            }

            if (!Directory.Exists(context.RepositoryFolder))
            {
                throw new DirectoryNotFoundException(
                    "Repository directory '" + context.RepositoryFolder + "' for services does not exists");
            }

            var repoServices = new Dictionary<string, List<RepoServicesData>>();
            var folders = Directory.GetDirectories(context.RepositoryFolder, "*");

            foreach (var folder in folders)
            {
                var directoryInfo = new DirectoryInfo(folder);
                repoServices.Add(directoryInfo.Name, new List<RepoServicesData>());
                var versions = Directory.GetDirectories(folder, "*");
                foreach (var versionDirectoryinfo in versions.Select(version => new DirectoryInfo(version)))
                {
                    int parsedVersion;
                    if (int.TryParse(versionDirectoryinfo.Name, out parsedVersion))
                    {
                        repoServices[directoryInfo.Name].Add(new RepoServicesData
                        {
                            Version = parsedVersion,
                            CreatedTime = versionDirectoryinfo.CreationTimeUtc
                        });
                    }
                }
            }

            return new GetAllRepoServicesReturnData { RepoServices = repoServices };
        }

        public string Session { get; set; }

        public string ApiKey { get; set; }
    }
}
