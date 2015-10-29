namespace Chains.UnitTests.Classes.Security
{
    using System.Security;
    using Chains.Play.Modules;
    using Chains.Play.Security;

    public sealed class SecurityModule : Chain<SecurityModule>, IModuleRequirement
    {
        public static string ActiveSession = "1234";

        private bool IsValid(IAuthorizableAction action)
        {
            return (action.Session == ActiveSession);
        }

        public bool CanExecute(object action)
        {
            var securableAction = action as IAuthorizableAction;
            if (securableAction == null)
            {
                return true;
            }

            if (!IsValid(securableAction))
            {
                throw new SecurityException("Access denied.");
            }

            return true;
        }
    }
}
