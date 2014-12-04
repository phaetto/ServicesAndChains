namespace Chains.Play.AppDomains
{
    using System;

    internal sealed class RemoteReplayProxy : MarshalByRefObject
    {
        private readonly ExecutionChain replayChain;

        public RemoteReplayProxy(string typeName)
        {
            replayChain = new ExecutionChain(typeName);
        }

        public ExecutableActionSpecification Play(ExecutableActionSpecification specification)
        {
            var executionResultContext = replayChain.Do(new ExecuteActionFromSpecification(specification));

            var remotableAction = replayChain.LastExecutedAction as IRemotable;

            if (remotableAction != null)
            {
                return executionResultContext.ToSpecification();
            }

            var reproducibleAction = replayChain.LastExecutedAction as IReproducible;

            if (reproducibleAction != null)
            {
                return reproducibleAction.GetInstanceSpec();
            }

            return null;
        }
    }
}
