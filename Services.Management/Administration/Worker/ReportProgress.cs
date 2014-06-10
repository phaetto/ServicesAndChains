namespace Services.Management.Administration.Worker
{
    using System;
    using System.Collections.Generic;
    using Chains.Play;
    using Services.Management.Administration.Server;

    public sealed class ReportProgress : RemotableActionWithData<ReportProgressData, ReportProgressReturnData, AdministrationContext>
    {
        public ReportProgress(ReportProgressData data)
            : base(data)
        {
        }

        protected override ReportProgressReturnData ActRemotely(AdministrationContext context)
        {
            var key = Data.StartData.Id;

            if (!context.ReportData.ContainsKey(key))
            {
                context.ReportData.Add(
                    key,
                    new WorkUnitReportData
                    {
                        Errors = new List<Exception>(),
                        Log = string.Empty,
                        AdviceGiven = AdviceState.Continue
                    });
            }

            var workUnitReportData = context.ReportData[key];

            workUnitReportData.StartedTime = Data.StartedTime;
            workUnitReportData.LastTimeContacted = DateTime.UtcNow;
            workUnitReportData.Uptime = DateTime.UtcNow - Data.StartedTime;
            workUnitReportData.StartData = Data.StartData;
            workUnitReportData.WorkerState = WorkUnitState.Running;
            workUnitReportData.PerformanceData = Data.PerformanceData;

            if (Data.LastError != null)
            {
                workUnitReportData.Errors.Add(Data.LastError);
            }

            if (!string.IsNullOrWhiteSpace(Data.AdditionalLog))
            {
                workUnitReportData.Log = Data.AdditionalLog + workUnitReportData.Log;
            }

            var reportResult = new ReportProgressReturnData
                               {
                                   Advice = workUnitReportData.AdviceGiven
                               };

            if (workUnitReportData.AdviceGiven == AdviceState.Restart)
            {
                workUnitReportData.WorkerState = WorkUnitState.Restarting;
            }

            // Must always put it back to continue
            workUnitReportData.AdviceGiven = AdviceState.Continue;

            context.ReportData[key] = workUnitReportData;

            return reportResult;
        }
    }
}
