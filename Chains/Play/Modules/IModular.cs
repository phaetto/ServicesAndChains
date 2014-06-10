namespace Chains.Play.Modules
{
    using System.Collections.Generic;

    public interface IModular
    {
        List<AbstractChain> Modules { get; set; }
    }
}
