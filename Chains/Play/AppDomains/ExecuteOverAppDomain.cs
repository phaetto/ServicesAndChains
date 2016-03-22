namespace Chains.Play.AppDomains
{
    using System;

    public class ExecuteOverAppDomain<T>
        : IChainableAction<AppDomainExecutionChain, T>
    {
        private readonly ExecutableActionSpecification specification;

        public ExecuteOverAppDomain(ExecutableActionSpecification specification)
        {
            this.specification = specification;
        }

        public ExecuteOverAppDomain(IRemotable remotableAction) : this(remotableAction as IReproducible)
        {
        }

        protected ExecuteOverAppDomain(IReproducible reproducibleAction)
        {
            specification = reproducibleAction.GetInstanceSpec();
        }

        public T Act(AppDomainExecutionChain context)
        {
            var specResult = context.Proxy.Play(specification);

            if (typeof(T) == typeof(AppDomainExecutionChain))
            {
                return (T)Convert.ChangeType(context, typeof(T));
            }

            if (specResult.DataType == typeof(bool).FullName)
            {
                return default(T);
            }

            return (T)Convert.ChangeType(specResult.Data, typeof(T));
        }
    }

    public class ExecuteOverAppDomain : ExecuteOverAppDomain<AppDomainExecutionChain>
    {
        public ExecuteOverAppDomain(ExecutableActionSpecification specification)
            : base(specification)
        {
        }

        public ExecuteOverAppDomain(IReproducible reproducibleAction)
            : base(reproducibleAction)
        {
        }
    }
}
