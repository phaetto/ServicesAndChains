﻿namespace Services.Management.Administration.Server.LastWellKnownConfiguration
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

            for (var version = workerData.Version; version > 0; --version)
            {
                dataLwkcFolder = context.Parent.DataFolder + LastWellKnownConfigurationContext.ConfigurationFolderName
                                 + Path.DirectorySeparatorChar
                                 + LastWellKnownConfigurationContext.AssemblyBlendingFolderName
                                 + Path.DirectorySeparatorChar + workerData.ServiceName + Path.DirectorySeparatorChar
                                 + version + Path.DirectorySeparatorChar
                                 + CreateLastWellKnownConfigurationSnapshot.RemoveTypeSuffix(workerData.ContextType)
                                 + Path.DirectorySeparatorChar;

                if (Directory.Exists(dataLwkcFolder))
                {
                    break;
                }

                if (version == 1)
                {
                    throw new InvalidOperationException("There was no available last well-known configuration version to restore.");
                }
            }

            var destinationPath = context.Parent.ServicesFolder + workerData.ServiceName + Path.DirectorySeparatorChar
                                  + workerData.Version + Path.DirectorySeparatorChar + workerData.Id
                                  + Path.DirectorySeparatorChar;

            if (Directory.Exists(destinationPath))
            {
                PrepareWorkerProcessFiles.DeleteFolderAndFiles(destinationPath);
            }

            PrepareWorkerProcessFiles.CopyFiles(dataLwkcFolder, destinationPath);

            workerData.FilesRepository = LastWellKnownConfigurationContext.LastWellKnownConfigurationFilesRepository;

            return context;
        }
    }
}
