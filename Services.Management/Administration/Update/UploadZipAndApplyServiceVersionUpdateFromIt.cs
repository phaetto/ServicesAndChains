namespace Services.Management.Administration.Update
{
    using System;
    using System.IO;
    using Chains;
    using Chains.Play;
    using Chains.Play.Security;
    using Services.Management.Administration.Server;

    public sealed class UploadZipAndApplyServiceVersionUpdateFromIt : ReproducibleWithData<FileUploadToAdminData>,
        IChainableAction<AdministrationContext, AdministrationContext>,
        IAuthorizableAction, IApplicationAuthorizableAction
    {
        public UploadZipAndApplyServiceVersionUpdateFromIt(FileUploadToAdminData data)
            : base(data)
        {
        }

        public AdministrationContext Act(AdministrationContext context)
        {
            if (Data.FileData == null || Data.FileData.Length == 0)
            {
                throw new InvalidOperationException("The file data cannot be empty.");
            }

            var tempFile = string.Format("{0}-temp.zip", Data.ServiceName);
            File.WriteAllBytes(tempFile, Data.FileData);

            context.Do(
                new ApplyServiceVersionUpdateFromZip(
                    new UpdateWorkUnitData
                    {
                        ServiceName = Data.ServiceName,
                        UpdateFolderOrFile = tempFile
                    }));

            File.Delete(tempFile);

            return context;
        }

        public string Session { get; set; }

        public string ApiKey { get; set; }
    }
}
