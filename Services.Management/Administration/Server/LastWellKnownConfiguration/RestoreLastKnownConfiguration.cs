namespace Services.Management.Administration.Server.LastWellKnownConfiguration
{
    using System;
    using System.IO;
    using Chains;
    using Services.Management.Administration.Worker;

    internal sealed class RestoreLastKnownConfiguration : IChainableAction<LastWellKnownConfigurationContext, LastWellKnownConfigurationContext>
    {
        private readonly StartWorkerData workerData;

        public RestoreLastKnownConfiguration(StartWorkerData workerData)
        {
            this.workerData = workerData;
        }

        public LastWellKnownConfigurationContext Act(LastWellKnownConfigurationContext context)
        {
            var dataLwkcFolder = string.Empty;

            var destinationPath = context.Parent.ServicesFolder + workerData.ServiceName + Path.DirectorySeparatorChar
                                  + workerData.Version + Path.DirectorySeparatorChar + workerData.Id
                                  + Path.DirectorySeparatorChar;

            for (var n = workerData.Version; n > 0; --n)
            {
                dataLwkcFolder = context.Parent.DataFolder + LastWellKnownConfigurationContext.ConfigurationFolderName
                                 + Path.DirectorySeparatorChar
                                 + LastWellKnownConfigurationContext.AssemblyBlendingFolderName
                                 + Path.DirectorySeparatorChar + workerData.ServiceName + Path.DirectorySeparatorChar
                                 + workerData.Version + Path.DirectorySeparatorChar;

                if (Directory.Exists(destinationPath))
                {
                    break;
                }

                if (n == 1)
                {
                    throw new InvalidOperationException("There was no available last well know configuration version to restore.");
                }
            }

            if (Directory.Exists(destinationPath))
            {
                PrepareWorkerProcessFiles.DeleteFolderAndFiles(destinationPath);
            }

            PrepareWorkerProcessFiles.CopyFiles(dataLwkcFolder, destinationPath);

            return context;
        }
    }
}
