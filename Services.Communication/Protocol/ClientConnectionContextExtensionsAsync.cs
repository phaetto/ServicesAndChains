﻿namespace Services.Communication.Protocol
{
    using System.Threading.Tasks;
    using Chains;
    using Chains.Play;

    public static class ClientConnectionContextExtensionsAsync
    {
        public static Task<TResultType> Do<T, TResultType>(
            this Task<ClientConnectionContext> context,
            IRemotableAction<T, TResultType> action)
            where T : Chain<T>
        {
            return context.ContinueWith(x => x.Do(new Send<TResultType>(action))).Unwrap();
        }

        public static Task<ClientConnectionContext> Do<T, TResultType>(
            this Task<ClientConnectionContext> context,
            IReproducible action)
            where TResultType : Chain<TResultType>
            where T : Chain<T>
        {
            return context.Do(new Send(action));
        }

        public static Task<ClientConnectionContext> Do(this Task<ClientConnectionContext> context, IReproducible action)
        {
            return context.Do(new Send(action));
        }

        public static Task<TResultType> Do<TResultType>(
            this Task<ClientConnectionContext> context,
            params ExecutableActionSpecification[] actionSpecifications)
        {
            return context.Do(new Send<TResultType>(actionSpecifications));
        }

        public static Task<ClientConnectionContext> Do(
            this Task<ClientConnectionContext> context,
            params ExecutableActionSpecification[] actionSpecifications)
        {
            return context.Do(new Send(actionSpecifications));
        }
    }
}
