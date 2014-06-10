namespace Chains.Play.Debug
{
    using System.Diagnostics;
    using Chains.Play.Security;

    public sealed class BreakDebugger : Reproducible,
        IChainableAction<DebugContext, DebugContext>,
        IApplicationAuthorizableAction,
        IAuthorizableAction
    {
        public DebugContext Act(DebugContext context)
        {
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }

            return context;
        }

        public string ApiKey { get; set; }

        public string Session { get; set; }
    }
}
