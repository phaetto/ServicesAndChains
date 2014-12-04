namespace Chains.Play.Modules
{
    using System.Linq;

    public sealed class SupportsModule : RemotableActionWithSerializableData<string, bool, IModular>
    {
        public SupportsModule(string data)
            : base(data)
        {
        }

        public override bool Act(IModular context)
        {
            var type = ExecutionChain.FindType(Data);

            if (type == null)
            {
                return false;
            }

            if (context.Modules.Any(
                module => module.GetType() == type
                    || module.GetType().IsSubclassOf(type)
                    || module.GetType().GetInterface(type.FullName) != null))
            {
                return true;
            }
            
            return (context.GetType().IsSubclassOf(type) || context.GetType() == type);
        }
    }
}
