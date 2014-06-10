namespace Services.Management.Administration.Executioner
{
    using Chains.Play.Security;
    using Services.Management.Administration.Worker;

    internal class WorkerDataSessionAndApiKey : StartWorkerData, IAuthorizableAction, IApplicationAuthorizableAction
    {
        public string Session { get; set; }

        public string ApiKey { get; set; }

        public override int DataStructureVersionNumber
        {
            get
            {
                return 1;
            }
        }
    }
}
