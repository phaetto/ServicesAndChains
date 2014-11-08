namespace Services.Communication.Protocol
{
    using System;
    using Chains.Play;

    public class ProtocolServerLogic
    {
        private readonly string ContextTypeName;
        private readonly Func<ExecutableActionSpecification[], bool> OnBeforeExecute;
        private readonly Action<dynamic> OnAfterExecute;
        internal readonly bool NewInstanceForEachRequest;

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

        public string ReadFromStreamAndPlayWithUniqueInstance(string stream)
        {
            var replayChainLocal = new ExecutionChain(ContextTypeName);

            var actionSpecifications = Deserialize(stream);

            if (actionSpecifications.Length == 0)
            {
                return null;
            }

            return ApplyDataAndReturnNoLockWithUnique(replayChainLocal, actionSpecifications);
        }

        public string ReadFromStreamAndPlay(string stream)
        {
            var actionSpecifications = Deserialize(stream);

            if (actionSpecifications.Length == 0)
            {
                return null;
            }

            return ApplyDataAndReturnNoLock(actionSpecifications);
        }

        public string ApplyDataAndReturnNoLock(ExecutableActionSpecification[] actionSpecifications)
        {
            if (OnBeforeExecute == null || OnBeforeExecute(actionSpecifications))
            {
                string dataToReturn;

                try
                {
                    foreach (var actionSpecification in actionSpecifications)
                    {
                        replayChain.Do(new ExecuteActionFromSpecification(actionSpecification));
                    }

                    if (replayChain.LastExecutedAction is IRemotable)
                    {
                        dataToReturn = Serialize(replayChain.LastExecutedAction as IRemotable);
                    }
                    else
                    {
                        dataToReturn = DefaultSerializedValue();
                    }

                    if (OnAfterExecute != null)
                    {
                        OnAfterExecute(replayChain.CurrentContext);
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

        public string ApplyDataAndReturnNoLockWithUnique(ExecutionChain replayChainInstance, ExecutableActionSpecification[] actionSpecifications)
        {
            if (OnBeforeExecute == null || OnBeforeExecute(actionSpecifications))
            {
                string dataToReturn;

                try
                {
                    foreach (var actionSpecification in actionSpecifications)
                    {
                        replayChainInstance.Do(new ExecuteActionFromSpecification(actionSpecification));
                    }

                    if (replayChainInstance.LastExecutedAction is IRemotable)
                    {
                        dataToReturn = Serialize(replayChainInstance.LastExecutedAction as IRemotable);
                    }
                    else
                    {
                        dataToReturn = DefaultSerializedValue();
                    }

                    if (OnAfterExecute != null)
                    {
                        OnAfterExecute(replayChainInstance.CurrentContext);
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

        public string ApplyDataAndReturn(ExecutableActionSpecification[] actionSpecifications)
        {
            if (OnBeforeExecute == null || OnBeforeExecute(actionSpecifications))
            {
                string dataToReturn;

                try
                {
                    lock (replayChain)
                    {
                        foreach (var actionSpecification in actionSpecifications)
                        {
                            replayChain.Do(new ExecuteActionFromSpecification(actionSpecification));
                        }

                        if (replayChain.LastExecutedAction is IRemotable)
                        {
                            dataToReturn = Serialize(replayChain.LastExecutedAction as IRemotable);
                        }
                        else
                        {
                            dataToReturn = DefaultSerializedValue();
                        }
                    }

                    if (OnAfterExecute != null)
                    {
                        OnAfterExecute(replayChain.CurrentContext);
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

        public ExecutableActionSpecification[] Deserialize(string data)
        {
            return DeserializableSpecification<ExecutableActionSpecification>.DeserializeManyFromJson(data);
        }

        public string Serialize(IRemotable action)
        {
            return action.ReturnData.SerializeToJson();
        }

        public string Serialize(Exception ex)
        {
            return new ExecutableActionSpecification
            {
                Data = ex,
                DataType = ex.GetType().FullName
            }.SerializeToJson();
        }

        public string DefaultSerializedValue()
        {
            return new ExecutableActionSpecification
            {
                Data = true,
                DataType = typeof(bool).FullName
            }.SerializeToJson();
        }
    }
}
