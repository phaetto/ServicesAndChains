namespace Services.Management.Administration.Worker
{
    using Chains;
    using Chains.Play;
    using Chains.Play.Security;

    public class CloseService : Reproducible,
        IChainableAction<WorkUnitContext, WorkUnitContext>,
        IAuthorizableAction, IApplicationAuthorizableAction
    {
        public WorkUnitContext Act(WorkUnitContext context)
        {
            context.Stop();

            return context;
        }

        public string Session { get; set; }

        public string ApiKey { get; set; }
    }
}
