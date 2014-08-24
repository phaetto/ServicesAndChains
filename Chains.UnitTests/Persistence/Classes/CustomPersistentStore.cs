namespace Chains.UnitTests.Persistence.Classes
{
    using System;
    using System.Collections.Generic;
    using Chains.Persistence;
    using Chains.Play;

    public class CustomPersistentStore<T> : IPersistentStore<T>
        where T : SerializableSpecificationWithId
    {
        public static Dictionary<string, List<ExecutableActionSpecification>> memoryStore =
            new Dictionary<string, List<ExecutableActionSpecification>>();
        public static Dictionary<string, DateTime> memoryStoreDateTimes = new Dictionary<string, DateTime>();

        public bool SnapshotExists(T data)
        {
            return memoryStoreDateTimes.ContainsKey(data.Id);
        }

        public DateTime GetLastActionEventTime(T data)
        {
            return memoryStoreDateTimes.ContainsKey(data.Id) ? memoryStoreDateTimes[data.Id] : DateTime.MinValue;
        }

        public T LoadSnapshot(T data)
        {
            return null;
        }

        public void SaveSnapshot(T data)
        {
        }

        public ExecutableActionSpecification[] LoadActionEvents(T data)
        {
            if (memoryStore.ContainsKey(data.Id))
            {
                return memoryStore[data.Id].ToArray();
            }

            return new ExecutableActionSpecification[0];
        }

        public void CreateActionEvent(T data)
        {
            if (!memoryStore.ContainsKey(data.Id))
            {
                memoryStore.Add(data.Id, new List<ExecutableActionSpecification>());
                memoryStoreDateTimes.Add(data.Id, DateTime.UtcNow);
            }
        }

        public void AppendActionEvent(T data, ExecutableActionSpecification executableActionSpecification)
        {
            if (memoryStore.ContainsKey(data.Id))
            {
                memoryStore[data.Id].Add(executableActionSpecification);
                memoryStoreDateTimes[data.Id] = DateTime.UtcNow;
            }
        }

        public void DeleteActionEvent(T data)
        {
            if (memoryStore.ContainsKey(data.Id))
            {
                memoryStore.Remove(data.Id);
                memoryStoreDateTimes.Remove(data.Id);
            }
        }
    }
}
