namespace Services.Communication.Protocol
{
    using System;
    using Chains.Play;

    public class ProtocolServerLogic
    {
        private readonly string ContextTypeName;
        private readonly Func<ExecutableActionSpecification[], bool> OnBeforeExecute;
        private readonly Action<dynamic> OnAfterExecute;
        private readonly bool NewInstanceForEachRequest;

        private ExecutionChain replayChain;

        public ProtocolServerLogic(
            object contextObject,
            Func<ExecutableActionSpecification[], bool> onBeforeExecute = null,
            Action<dynamic> onAfterExecute = null,
            bool newInstanceForEachRequest = false)
        {
            ContextTypeName = contextObject.GetType().AssemblyQualifiedName;
            this.OnBeforeExecute = onBeforeExecute;
            this.OnAfterExecute = onAfterExecute;
            this.NewInstanceForEachRequest = newInstanceForEachRequest;
            replayChain = new ExecutionChain(contextObject);
        }

        public ProtocolServerLogic(
            string contextTypeName,
            Func<ExecutableActionSpecification[], bool> onBeforeExecute = null,
            Action<dynamic> onAfterExecute = null,
            bool newInstanceForEachRequest = false)
        {
            this.ContextTypeName = contextTypeName;
            this.OnBeforeExecute = onBeforeExecute;
            this.OnAfterExecute = onAfterExecute;
            this.NewInstanceForEachRequest = newInstanceForEachRequest;
            replayChain = new ExecutionChain(contextTypeName);
        }

        public string ReadFromStreamAndPlay(string data, bool applyLock = false)
        {
            var actionSpecifications = Deserialize(data);

            if (actionSpecifications.Length == 0)
            {
                return null;
            }

            if (NewInstanceForEachRequest)
            {
                var replayChainLocal = new ExecutionChain(ContextTypeName);

                return ApplyDataAndReturn(replayChainLocal, actionSpecifications, applyLock);
            }

            return ApplyDataAndReturn(replayChain, actionSpecifications, applyLock);
        }

        public string ReadFromStreamAndPlay(ExecutableActionSpecification[] actionSpecifications, bool applyLock = false)
        {
            if (actionSpecifications.Length == 0)
            {
                return null;
            }

            if (NewInstanceForEachRequest)
            {
                var replayChainLocal = new ExecutionChain(ContextTypeName);

                return ApplyDataAndReturn(replayChainLocal, actionSpecifications, applyLock);
            }

            return ApplyDataAndReturn(replayChain, actionSpecifications, applyLock);
        }

        public string Serialize(Exception ex)
        {
            return new ExecutableActionSpecification
            {
                Data = ex,
                DataType = ex.GetType().FullName
            }.SerializeToJson();
        }

        private string ApplyDataAndReturn(ExecutionChain chainInstance, ExecutableActionSpecification[] actionSpecifications, bool applyLock = false)
        {
            if (OnBeforeExecute == null || OnBeforeExecute(actionSpecifications))
            {
                string dataToReturn;

                try
                {
                    if (applyLock)
                    {
                        lock (chainInstance)
                        {
                            dataToReturn = ApplyDataOnExecutionChain(chainInstance, actionSpecifications);
                        }
                    }
                    else
                    {
                        dataToReturn = ApplyDataOnExecutionChain(chainInstance, actionSpecifications);
                    }

                    if (OnAfterExecute != null)
                    {
                        OnAfterExecute(chainInstance.CurrentContext);
                    }
                }
                catch (Exception ex)
                {
                    return Serialize(ex);
                }

                return dataToReturn;
            }

            return null;
        }

        private string ApplyDataOnExecutionChain(ExecutionChain chainInstance, ExecutableActionSpecification[] actionSpecifications)
        {
            string dataToReturn;

            foreach (var actionSpecification in actionSpecifications)
            {
                chainInstance.Do(new ExecuteActionFromSpecification(actionSpecification));
            }

            if (replayChain.LastExecutedAction is IRemotable)
            {
                dataToReturn = Serialize(chainInstance.LastExecutedAction as IRemotable);
            }
            else
            {
                dataToReturn = DefaultSerializedValue();
            }

            return dataToReturn;
        }

        private ExecutableActionSpecification[] Deserialize(string data)
        {
            return DeserializableSpecification<ExecutableActionSpecification>.DeserializeManyFromJson(data);
        }

        private string Serialize(IRemotable action)
        {
            return action.ReturnData.SerializeToJson();
        }

        private string DefaultSerializedValue()
        {
            return new ExecutableActionSpecification
            {
                Data = true,
                DataType = typeof(bool).FullName
            }.SerializeToJson();
        }
    }
}
