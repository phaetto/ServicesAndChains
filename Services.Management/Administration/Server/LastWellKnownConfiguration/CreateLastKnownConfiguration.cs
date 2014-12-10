namespace Services.Management.Administration.Server.LastWellKnownConfiguration
{
    using System.IO;
    using Chains;
    using Services.Management.Administration.Worker;

    internal sealed class CreateLastWellKnownConfigurationSnapshot : IChainableAction<LastWellKnownConfigurationContext, LastWellKnownConfigurationContext>
    {
        private readonly string serviceId;

        public CreateLastWellKnownConfigurationSnapshot(string serviceId)
        {
            this.serviceId = serviceId;
        }

        public LastWellKnownConfigurationContext Act(LastWellKnownConfigurationContext context)
        {
            var reports = context.Parent.Do(new GetReportedData());

            if (!reports.Reports.ContainsKey(serviceId))
            {
                return context;
            }

            var reportData = reports.Reports[serviceId];

            if (reportData.WorkerState == WorkUnitState.Abandoned
                || reportData.StartData.ServiceName == AdministrationContext.GeneralServiceName
                || reportData.StartData.FilesRepository == LastWellKnownConfigurationContext.LastWellKnownConfigurationFilesRepository)
            {
                return context;
            }

            var dataLwkcFolder = context.Parent.DataFolder + LastWellKnownConfigurationContext.ConfigurationFolderName
                                 + Path.DirectorySeparatorChar
                                 + LastWellKnownConfigurationContext.AssemblyBlendingFolderName
                                 + Path.DirectorySeparatorChar + reportData.StartData.ServiceName
                                 + Path.DirectorySeparatorChar + reportData.StartData.Version
                                 + Path.DirectorySeparatorChar + RemoveTypeSuffix(reportData.StartData.ContextType)
                                 + Path.DirectorySeparatorChar;

            var sourcePath = context.Parent.ServicesFolder + reportData.StartData.ServiceName + Path.DirectorySeparatorChar
                             + reportData.StartData.Version + Path.DirectorySeparatorChar + reportData.StartData.Id
                             + Path.DirectorySeparatorChar;

            if (Directory.Exists(dataLwkcFolder))
            {
                PrepareWorkerProcessFiles.DeleteFolderAndFiles(dataLwkcFolder);
            }

            PrepareWorkerProcessFiles.CopyFiles(sourcePath, dataLwkcFolder);

            return context;
        }

        internal static string RemoveTypeSuffix(string typename)
        {
            var indexOfComma = typename.IndexOf(',');
            if (indexOfComma > -1)
            {
                return typename.Substring(0, indexOfComma);
            }

            return typename;
        }
    }
}
