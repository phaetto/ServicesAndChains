namespace Chains.Play.Modules
{
    using System;
    using System.Linq;

    public sealed class IsModuleEnabled : RemotableActionWithSerializableData<string, bool, IModular>
    {
        public IsModuleEnabled(string data)
            : base(data)
        {
        }

        protected override bool ActRemotely(IModular context)
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

            var modulesFound = context.Modules.Where(module => module.GetType() == type && module is IOptionalModule).Cast<IOptionalModule>();

            if (modulesFound.Any(module => module.IsEnabled))
            {
                return true;
            }
            
            return false;
        }
    }
}
