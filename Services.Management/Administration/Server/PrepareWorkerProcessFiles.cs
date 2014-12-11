namespace Services.Management.Administration.Server
{
    using System;
    using System.IO;
    using System.Linq;
    using Chains.Play;
    using Chains.Play.Security;
    using Services.Management.Administration.Worker;

    public sealed class PrepareWorkerProcessFiles : RemotableActionWithData<StartWorkerData, StartWorkerData, AdministrationContext>,
        IAuthorizableAction, IApplicationAuthorizableAction
    {
        private readonly string executionerRepo;

        public PrepareWorkerProcessFiles(StartWorkerData data)
            : base(data)
        {
            executionerRepo = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar;
        }

        public override StartWorkerData Act(AdministrationContext context)
        {
            var repoServicePath = context.RepositoryFolder + Data.ServiceName + Path.DirectorySeparatorChar;
            if (Data.Version == 0 && Directory.Exists(repoServicePath))
            {
                Data.Version =
                    context.Do(new GetAllRepoServices())
                        .RepoServices[Data.ServiceName]
                        .OrderByDescending(x => x.Version).First().Version;
            }

            var id = string.IsNullOrWhiteSpace(Data.Id) ? Guid.NewGuid().ToString() : Data.Id;
            var sourcePath = repoServicePath + Data.Version + Path.DirectorySeparatorChar;
            var destinationPath = context.ServicesFolder + Data.ServiceName + Path.DirectorySeparatorChar
                + Data.Version + Path.DirectorySeparatorChar + id + Path.DirectorySeparatorChar;

            if (context.ReportData.ContainsKey(id))
            {
                if (context.ReportData[id].WorkerState == WorkUnitState.Running)
                {
                    throw new InvalidOperationException(
                        "There is already a service with id '" + id + "'. Id should be unique.");
                }

                context.ReportData[id].AdviceGiven = AdviceState.Continue;
            }

            if (Directory.Exists(destinationPath))
            {
                DeleteFolderAndFiles(destinationPath);
            }

            if (Directory.Exists(sourcePath))
            {
                CopyFiles(sourcePath, destinationPath);
            }

            CopyFilesWithoutRepoAndServices(context, executionerRepo, destinationPath);

            Data.Id = id;
            Data.HostProcessFileName = context.HostProcessName;
            Data.FilesRepository = AdministrationContext.DefaultFilesRepository;

            return Data;
        }

        internal static void CopyFilesWithoutRepoAndServices(AdministrationContext context, string SourcePath, string DestinationPath)
        {
            if (!Directory.Exists(SourcePath))
            {
                throw new DirectoryNotFoundException("Source directory '" + SourcePath + "' does not exists");
            }

            var allDirectories = Directory.GetDirectories(SourcePath, "*", SearchOption.AllDirectories);
            foreach (var dirPath in allDirectories)
            {
                if (!dirPath.StartsWith(context.RepositoryFolder) && !dirPath.StartsWith(context.ServicesFolder))
                {
                    Directory.CreateDirectory(dirPath.Replace(SourcePath, DestinationPath));
                }
            }

            var allFiles = Directory.GetFiles(SourcePath, "*.*", SearchOption.AllDirectories);
            foreach (var file in allFiles)
            {
                if (!file.StartsWith(context.RepositoryFolder) && !file.StartsWith(context.ServicesFolder))
                {
                    var newFile = file.Replace(SourcePath, DestinationPath);

                    if (File.Exists(newFile))
                    {
                        File.Delete(newFile);
                    }

                    File.Copy(file, newFile);
                }
            }
        }

        internal static void CopyFiles(string SourcePath, string DestinationPath)
        {
            if (!Directory.Exists(SourcePath))
            {
                throw new DirectoryNotFoundException("Source directory '" + SourcePath + "' does not exists");
            }

            if (!Directory.Exists(DestinationPath))
            {
                Directory.CreateDirectory(DestinationPath);
            }

            var allDirectories = Directory.GetDirectories(SourcePath, "*", SearchOption.AllDirectories);
            foreach (var dirPath in allDirectories)
            {
                Directory.CreateDirectory(dirPath.Replace(SourcePath, DestinationPath));
            }

            var allFiles = Directory.GetFiles(SourcePath, "*.*", SearchOption.AllDirectories);
            foreach (var file in allFiles)
            {
                var newFile = file.Replace(SourcePath, DestinationPath);

                if (File.Exists(newFile))
                {
                    File.Delete(newFile);
                }

                File.Copy(file, newFile);
            }
        }

        internal static void DeleteFiles(string folder)
        {
            var folderToDelete = new DirectoryInfo(folder);

            foreach (var file in folderToDelete.GetFiles())
            {
                file.Delete();
            }
        }

        internal static void DeleteFolderAndFiles(string folder)
        {
            var folderToDelete = new DirectoryInfo(folder);

            foreach (var file in folderToDelete.GetFiles())
            {
                file.Delete();
            }

            foreach (var dir in folderToDelete.GetDirectories())
            {
                dir.Delete(true);
            }

            folderToDelete.Delete();
        }

        internal static long MeasureFilesDiskSize(string folder)
        {
            var folderToMeasure = new DirectoryInfo(folder);

            return folderToMeasure.GetFiles().Select(x => x.Length).Aggregate((x, y) => x + y);
        }

        public string Session { get; set; }

        public string ApiKey { get; set; }
    }
}
