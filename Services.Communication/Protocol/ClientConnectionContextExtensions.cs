namespace Services.Communication.Protocol
{
    using Chains;
    using Chains.Play;

    public static class ClientConnectionContextExtensions
    {
        public static TResultType Do<T, TResultType>(
            this ClientConnectionContext context,
            IRemotableAction<T, TResultType> action)
            where TResultType : class
            where T : Chain<T>
        {
            return context.Do(new Send<TResultType>(action));
        }

        public static ClientConnectionContext Do<T, TResultType>(
            this ClientConnectionContext context,
            IReproducible action)
            where TResultType : Chain<TResultType>
            where T : Chain<T>
        {
            return context.Do(new Send(action));
        }

        public static ClientConnectionContext Do(this ClientConnectionContext context, IReproducible action)
        {
            return context.Do(new Send(action));
        }

        public static TResultType Do<TResultType>(
            this ClientConnectionContext context,
            params ExecutableActionSpecification[] actionSpecifications)
            where TResultType : class
        {
            return context.Do(new Send<TResultType>(actionSpecifications));
        }

        public static ClientConnectionContext Do(
            this ClientConnectionContext context,
            params ExecutableActionSpecification[] actionSpecifications)
        {
            return context.Do(new Send(actionSpecifications));
        }
    }
}
