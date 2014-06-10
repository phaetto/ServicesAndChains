namespace Chains.Play.Security.Provider
{
    public interface IAuthenticationGate
    {
        ProviderAuthenticationGateData GetAuthorizationData();
    }
}
