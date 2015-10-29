namespace Services.Communication.Protocol
{
    using Chains;
    using Chains.Play;

    public static class ClientConnectionContextExtensions
    {
        public static TResultType Do<T, TResultType>(
            this ClientConnectionContext context,
            IRemotableAction<T, TResultType> action)
        {
            return context.Do(new Send<TResultType>(action));
        }

        public static ClientConnectionContext Do<T, TResultType>(
            this ClientConnectionContext context,
            IReproducible action)
            where TResultType : Chain<TResultType>
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
        {
            return context.Do(new Send<TResultType>(actionSpecifications));
        }

        public static ClientConnectionContext Do(
            this ClientConnectionContext context,
            params ExecutableActionSpecification[] actionSpecifications)
        {
            return context.Do(new Send(actionSpecifications));
        }

        public static string Do(this ClientConnectionContext context, string rawInput)
        {
            return context.Do(new SendRaw(rawInput));
        }
    }
}
