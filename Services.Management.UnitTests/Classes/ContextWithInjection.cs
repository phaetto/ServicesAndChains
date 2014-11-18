namespace Services.Management.UnitTests.Classes
{
    using Chains;
    using Services.Management.Administration.Worker;

    public class ContextWithInjection : Chain<ContextWithInjection>
    {
        public readonly WorkUnitContext WorkUnitContext;

        public ContextWithInjection(WorkUnitContext workUnitContext)
        {
            this.WorkUnitContext = workUnitContext;
        }
    }
}
