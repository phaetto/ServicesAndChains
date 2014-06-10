namespace Chains.Play.Security.Provider
{
    public sealed class GetAuthenticationGateOptions : RemotableAction<ProviderAuthenticationGateData, IAuthenticationGate>
    {
        protected override ProviderAuthenticationGateData ActRemotely(IAuthenticationGate context)
        {
            return context.GetAuthorizationData();
        }
    }
}
