namespace Chains
{
    using System;

    public abstract class AbstractChain
    {
        public static bool IsMono => Type.GetType("Mono.Runtime") != null;
    }
}
