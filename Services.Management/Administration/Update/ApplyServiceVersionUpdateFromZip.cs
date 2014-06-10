namespace Services.Management.Administration.Update
{
    using System;
    using System.IO;
    using Chains;
    using Chains.Play;
    using Chains.Play.Security;
    using Ionic.Zip;
    using Services.Management.Administration.Server;

    public sealed class ApplyServiceVersionUpdateFromZip : ReproducibleWithData<UpdateWorkUnitData>,
        IChainableAction<AdministrationContext, AdministrationContext>,
        IAuthorizableAction, IApplicationAuthorizableAction
    {
        public ApplyServiceVersionUpdateFromZip(UpdateWorkUnitData data)
            : base(data)
        {
        }

        public AdministrationContext Act(AdministrationContext context)
        {
            if (string.IsNullOrEmpty(Data.UpdateFolderOrFile))
            {
                throw new InvalidOperationException("The file cannot be empty string.");
            }

            if (!File.Exists(Data.UpdateFolderOrFile))
            {
                throw new InvalidOperationException("The file '" + Data.UpdateFolderOrFile + "' does not exist.");
            }

            var tempDir = string.Format(
                "{0}{2}{1}-temp{2}",
                AppDomain.CurrentDomain.BaseDirectory,
                Data.ServiceName,
                Path.DirectorySeparatorChar);

            Directory.CreateDirectory(tempDir);

            using (var zipFile = ZipFile.Read(Data.UpdateFolderOrFile))
            {
                foreach (var zipEntry in zipFile)
                {
                    zipEntry.Extract(tempDir, ExtractExistingFileAction.OverwriteSilently);
                }
            }

            context.Do(
                new ApplyServiceVersionUpdate(
                    new UpdateWorkUnitData
                    {
                        ServiceName = Data.ServiceName,
                        UpdateFolderOrFile = tempDir
                    }));

            Directory.Delete(tempDir, true);

            return context;
        }

        public string Session { get; set; }

        public string ApiKey { get; set; }
    }
}
