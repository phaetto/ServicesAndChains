namespace Chains
{
    using System;

    public abstract class AbstractChain
    {
        public static bool IsMono => Type.GetType("Mono.Runtime") != null;

        protected Action<object> OnBeforeExecuteAction;
        protected Action<object> OnAfterExecuteAction;
    }
}
