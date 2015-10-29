namespace Chains.UnitTests.Classes.Security
{
    using System;
    using System.Security;
    using Chains.Play.Modules;
    using Chains.Play.Security;

    public sealed class SecurityModuleForOnlyAuthorizedActions : Chain<SecurityModuleForOnlyAuthorizedActions>, IModuleRequirement
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
                throw new InvalidOperationException("Access denied");
            }

            if (!IsValid(securableAction))
            {
                throw new SecurityException("Access denied.");
            }

            return true;
        }
    }
}
