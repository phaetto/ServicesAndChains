namespace Chains.UnitTests.Classes.Security
{
    using System.Collections.Generic;
    using Chains.Play.Modules;

    public class SecuredContextForTest : Chain<SecuredContextForTest>, IModular
    {
        public string contextVariable = null;

        public SecuredContextForTest()
        {
            Modules = new List<AbstractChain>();
            Modules.Add(new SecurityModule());
        }

        public List<AbstractChain> Modules { get; set; }
    }
}
