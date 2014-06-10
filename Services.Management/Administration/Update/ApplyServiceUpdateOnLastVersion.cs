namespace Services.Management.Administration.Update
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Chains;
    using Chains.Play;
    using Chains.Play.Security;
    using Services.Management.Administration.Server;

    public sealed class ApplyServiceUpdateOnLastVersion : ReproducibleWithData<UpdateWorkUnitData>,
        IChainableAction<AdministrationContext, AdministrationContext>,
        IAuthorizableAction, IApplicationAuthorizableAction
    {
        public ApplyServiceUpdateOnLastVersion(UpdateWorkUnitData data)
            : base(data)
        {
        }

        public AdministrationContext Act(AdministrationContext context)
        {
            var data = context.DoRemotable(new GetAllRepoServices()).RepoServices;

            if (!data.ContainsKey(Data.ServiceName))
            {
                data.Add(Data.ServiceName, new List<RepoServicesData>());
            }

            var lastVersion =
                data[Data.ServiceName].Select(x => x.Version).OrderByDescending(x => x).FirstOrDefault();

            var repositoryPath = context.RepositoryFolder + Data.ServiceName + Path.DirectorySeparatorChar
                + lastVersion + Path.DirectorySeparatorChar;

            PrepareWorkerProcessFiles.CopyFiles(context, Data.UpdateFolderOrFile, repositoryPath);

            return context;
        }

        public string Session { get; set; }

        public string ApiKey { get; set; }
    }
}
