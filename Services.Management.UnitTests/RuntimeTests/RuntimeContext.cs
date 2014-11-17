namespace Services.Management.UnitTests.RuntimeTests
{
    using Chains;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Services.Management.Administration.Worker;

    public sealed class RuntimeContext : Chain<RuntimeContext>
    {
        public RuntimeContext(WorkUnitContext workUnitContext)
        {
            Assert.IsNotNull(workUnitContext);
        }
    }
}
