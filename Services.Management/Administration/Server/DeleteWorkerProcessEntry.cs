namespace Services.Management.Administration.Server
{
    using System;
    using System.IO;
    using System.Linq;
    using Chains;
    using Chains.Play;
    using Chains.Play.Security;
    using Services.Management.Administration.Worker;

    public sealed class DeleteWorkerProcessEntry : ReproducibleWithSerializableData<string>,
        IChainableAction<AdministrationContext, AdministrationContext>,
        IAuthorizableAction, IApplicationAuthorizableAction
    {
        public DeleteWorkerProcessEntry(string data)
            : base(data)
        {
        }

        public AdministrationContext Act(AdministrationContext context)
        {
            if (context.ReportData.ContainsKey(Data))
            {
                var processPath = context.ServicesFolder + context.ReportData[Data].StartData.ServiceName
                    + Path.DirectorySeparatorChar + context.ReportData[Data].StartData.Version
                    + Path.DirectorySeparatorChar + Data + Path.DirectorySeparatorChar;

                if (context.ReportData[Data].WorkerState == WorkUnitState.Running)
                {
                    throw new InvalidOperationException("The service still runs and cannot be deleted.");
                }

                try
                {
                    Directory.Delete(processPath, true);
                }
                catch (IOException exception)
                {
                    context.LogException(exception);
                }

                context.ReportData.Remove(Data);
            }

            return context;
        }

        public string Session { get; set; }

        public string ApiKey { get; set; }
    }
}
