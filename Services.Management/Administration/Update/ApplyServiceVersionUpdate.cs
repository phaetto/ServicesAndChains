﻿namespace Services.Management.Administration.Update
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Chains;
    using Chains.Play;
    using Chains.Play.Security;
    using Services.Management.Administration.Server;

    public sealed class ApplyServiceVersionUpdate : ReproducibleWithData<UpdateWorkUnitData>,
        IChainableAction<AdministrationContext, AdministrationContext>,
        IAuthorizableAction, IApplicationAuthorizableAction
    {
        public ApplyServiceVersionUpdate(UpdateWorkUnitData data)
            : base(data)
        {
        }

        public AdministrationContext Act(AdministrationContext context)
        {
            var data = context.Do(new GetAllRepoServices()).RepoServices;

            if (!data.ContainsKey(Data.ServiceName))
            {
                data.Add(Data.ServiceName, new List<RepoServicesData>());
            }

            var lastVersion =
                data[Data.ServiceName].Select(x => x.Version).OrderByDescending(x => x).FirstOrDefault();

            // While you copy new implementation increment service number
            var repositoryPath = context.RepositoryFolder + Data.ServiceName + Path.DirectorySeparatorChar
                + (lastVersion + 1) + Path.DirectorySeparatorChar;

            PrepareWorkerProcessFiles.CopyFiles(context, Data.UpdateFolderOrFile, repositoryPath);

            return context;
        }

        public string Session { get; set; }

        public string ApiKey { get; set; }
    }
}
