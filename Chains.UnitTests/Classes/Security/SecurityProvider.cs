namespace Chains.UnitTests.Classes.Security
{
    using Chains.Play.Security.Provider;

    public sealed class SecurityProvider : Chain<SecurityProvider>, IAuthenticationGate
    {
        private readonly int port;

        public SecurityProvider(int port)
        {
            this.port = port;
        }

        public ProviderAuthenticationGateData GetAuthorizationData()
        {
            return new ProviderAuthenticationGateData
                   {
                       LoginUrl = "http://localhost:" + port + "/login",
                       LoginUrlMethod = LoginUrlMethod.PostRequest,
                       LoginUrlDataToSend = new[]
                                            {
                                                "username", "password"
                                            },
                   };
        }
    }
}
