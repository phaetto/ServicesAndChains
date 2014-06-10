namespace Chains.Play
{
    using Chains.Play.Security;

    public abstract class Reproducible : IReproducible
    {
        public ExecutableActionSpecification GetInstanceSpec()
        {
            var securable = this as IAuthorizableAction;
            var apiAction = this as IApplicationAuthorizableAction;

            return new ExecutableActionSpecification
                   {
                       Type = GetType().FullName,
                       Session = securable != null ? securable.Session : null,
                       ApiKey = apiAction != null ? apiAction.ApiKey : null
                   };
        }
    }
}