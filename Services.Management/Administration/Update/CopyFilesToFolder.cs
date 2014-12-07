namespace Services.Management.Administration.Update
{
    using System.IO;
    using Chains;
    using Chains.Play;
    using Chains.Play.Security;
    using Services.Management.Administration.Server;

    public sealed class CopyFilesToFolder : ReproducibleWithData<CopyFilesToFolderData>,
        IChainableAction<AdministrationContext, AdministrationContext>,
        IAuthorizableAction, IApplicationAuthorizableAction
    {
        public CopyFilesToFolder(CopyFilesToFolderData data)
            : base(data)
        {
        }

        public AdministrationContext Act(AdministrationContext context)
        {
            PrepareWorkerProcessFiles.CopyFiles(Path.GetDirectoryName(Data.File), Data.FolderToUpdate);

            return context;
        }

        public string Session { get; set; }

        public string ApiKey { get; set; }
    }
}
