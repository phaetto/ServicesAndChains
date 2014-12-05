namespace Chains.Play.Security.Provider
{
    public sealed class GetAuthenticationGateOptions : RemotableAction<ProviderAuthenticationGateData, IAuthenticationGate>
    {
        public override ProviderAuthenticationGateData Act(IAuthenticationGate context)
        {
            return context.GetAuthorizationData();
        }
    }
}
