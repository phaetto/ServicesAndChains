namespace Chains.Play.Modules
{
    using System;
    using System.Linq;
    using Chains.Play.Security;

    public sealed class EnableModule : ReproducibleWithSerializableData<string>,
        IChainableAction<IModular, IModular>,
        IApplicationAuthorizableAction,
        IAuthorizableAction
    {
        public EnableModule(string data)
            : base(data)
        {
        }

        public IModular Act(IModular context)
        {
            var type = ExecutionChain.FindType(Data);

            if (type == null)
            {
                throw new TypeLoadException("Type does not exist");
            }

            if (type.GetInterface(typeof(IOptionalModule).FullName) == null)
            {
                throw new NotSupportedException("IsModuleEnabled is not supported from the requested type");
            }

            var modulesFound =
                context.Modules.Where(module => module.GetType() == type && module is IOptionalModule)
                       .Cast<IOptionalModule>()
                       .Where(module => !module.IsEnabled);

            foreach (var module in modulesFound)
            {
                module.IsEnabled = true;
            }

            return context;
        }

        public string ApiKey { get; set; }

        public string Session { get; set; }
    }
}
