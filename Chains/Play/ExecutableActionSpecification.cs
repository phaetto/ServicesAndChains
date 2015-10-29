namespace Chains.Play
{
    using System;
    using Chains.Play.Security;

    [Serializable]
    public class ExecutableActionSpecification : SerializableSpecification
    {
        public string Type;

        public string DataType;

        public object Data;

        public string Session;

        public string ApiKey;

        public override int DataStructureVersionNumber
        {
            get
            {
                return 1;
            }
        }

        public T CreateFromSpec<T>()
        {
            if (string.IsNullOrWhiteSpace(Type))
            {
                throw new NullReferenceException("Type cannot be null nor a whitespace");
            }

            object generatedObject = null;

            if (Data == null)
            {
                generatedObject = ExecutionChain.CreateObjectWithParameters(Type);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(DataType))
                {
                    throw new NullReferenceException("DataType cannot be null nor a whitespace when data exist.");
                }

                generatedObject = ExecutionChain.CreateObjectWithParameters(Type, Data);
            }

            if (generatedObject == null)
            {
                return default(T);
            }

            var securable = generatedObject as IAuthorizableAction;
            var apiAction = generatedObject as IApplicationAuthorizableAction;

            if (securable != null)
            {
                securable.Session = Session;
            }

            if (apiAction != null)
            {
                apiAction.ApiKey = ApiKey;
            }

            return (T)generatedObject;
        }

        public object CreateFromSpec()
        {
            return CreateFromSpec<object>();
        }
    }
}
