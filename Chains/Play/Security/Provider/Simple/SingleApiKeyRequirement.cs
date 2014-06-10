namespace Chains.Play.Security.Provider.Simple
{
    using System.Security;
    using Chains.Play.Modules;

    public sealed class SingleApiKeyRequirement : Chain<SingleApiKeyRequirement>, IModuleRequirement
    {
        private readonly string apiKey;

        public SingleApiKeyRequirement(string apiKey)
        {
            this.apiKey = apiKey;
        }

        public bool CanExecute(object action)
        {
            var appSecurableAction = action as IApplicationAuthorizableAction;
            if (appSecurableAction == null)
            {
                return true;
            }

            if (apiKey != appSecurableAction.ApiKey)
            {
                throw new SecurityException("Access denied.");
            }

            return true;
        }
    }
}
