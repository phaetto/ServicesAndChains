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
                || reportData.StartData.ServiceName == AdministrationContext.GeneralServiceName)
            {
                return context;
            }

            var dataLwkcFolder = context.Parent.DataFolder + "LastWellKnownConfiguration" + Path.DirectorySeparatorChar
                                 + "AssemblyBlending" + Path.DirectorySeparatorChar + reportData.StartData.ServiceName
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
    }
}
