namespace Chains.Play
{
    using System;
    using System.Runtime.Serialization;
    using Chains.Play.Security;

    public abstract class ReproducibleWithSerializableData<TDataType> : IReproducible
    {
        public TDataType Data { get; set; }

        static ReproducibleWithSerializableData()
        {
            if (!typeof(TDataType).IsSerializable && !typeof(ISerializable).IsAssignableFrom(typeof(TDataType)))
            {
                throw new InvalidOperationException(
                    "A serializable Type is required to use ReproducibleAction::DataType");
            }
        }

        protected ReproducibleWithSerializableData(TDataType data)
        {
            Data = data;
        }

        public ExecutableActionSpecification GetInstanceSpec()
        {
            var secureAction = this as IAuthorizableAction;
            var apiAction = this as IApplicationAuthorizableAction;

            return new ExecutableActionSpecification
                   {
                       Data = Data,
                       DataType = Data != null ? Data.GetType().FullName : null,
                       Type = GetType().FullName,
                       Session = secureAction != null ? secureAction.Session : null,
                       ApiKey = apiAction != null ? apiAction.ApiKey : null
                   };
        }
    }
}