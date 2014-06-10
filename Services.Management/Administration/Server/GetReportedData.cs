namespace Services.Management.Administration.Server
{
    using System.Linq;
    using Chains.Play;
    using Chains.Play.Security;

    public sealed class GetReportedData : RemotableAction<GetReportedDataReturnData, AdministrationContext>,
        IAuthorizableAction, IApplicationAuthorizableAction
    {
        private const int MaximumLogSizeToSend = 10 * 1024;

        private const int MaximumErrorsToSend = 20;

        protected override GetReportedDataReturnData ActRemotely(AdministrationContext context)
        {
            var reportDataWithSomeOfTheLog = context.ReportData.ToDictionary(
                x => x.Key,
                x => new WorkUnitReportData()
                     {
                         AdviceGiven = x.Value.AdviceGiven,
                         Errors =
                             x.Value.Errors.Count > MaximumErrorsToSend
                                 ? x.Value.Errors.ToArray().Reverse().Take(MaximumErrorsToSend).Reverse().ToList()
                                 : x.Value.Errors,
                         ErrorCount = x.Value.Errors.Count,
                         LastTimeContacted = x.Value.LastTimeContacted,
                         PerformanceData = x.Value.PerformanceData,
                         StartData = x.Value.StartData,
                         StartedTime = x.Value.StartedTime,
                         Uptime = x.Value.Uptime,
                         WorkerState = x.Value.WorkerState,
                         Log =
                             x.Value.Log.Length > MaximumLogSizeToSend
                                 ? x.Value.Log.Substring(0, MaximumLogSizeToSend)
                                 : x.Value.Log,
                         LogSizeInCharacters = x.Value.Log.Length,
                     });

            return new GetReportedDataReturnData { Reports = reportDataWithSomeOfTheLog };
        }

        public string Session { get; set; }

        public string ApiKey { get; set; }
    }
}
