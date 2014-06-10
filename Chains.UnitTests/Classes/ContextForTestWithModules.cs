namespace Chains.UnitTests.Classes
{
    using System.Collections.Generic;
    using Chains.Play;
    using Chains.Play.Modules;

    public class ContextForTestWithModules : Chain<ContextForTestWithModules>, IModular
    {
        public string contextVariable = null;

        public ContextForTestWithModules()
        {
            Modules = new List<AbstractChain>();
            Modules.Add(new ContextForTest2());
        }

        public List<AbstractChain> Modules { get; set; }
    }
}
