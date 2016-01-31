namespace Chains.UnitTests.Persistence.Classes
{
    using System;
    using System.Collections.Generic;
    using Chains.Persistence;
    using Chains.Play;

    public class CustomPersistentStore<T> : IPersistentStore<T>
        where T : SerializableSpecificationWithId
    {
        public static Dictionary<string, List<ExecutableActionSpecification>> MemoryStore =
            new Dictionary<string, List<ExecutableActionSpecification>>();
        public static Dictionary<string, DateTime> MemoryStoreDateTimes = new Dictionary<string, DateTime>();

        public bool SnapshotExists(T data)
        {
            return MemoryStoreDateTimes.ContainsKey(data.Id);
        }

        public DateTime GetLastActionEventTime(T data)
        {
            return MemoryStoreDateTimes.ContainsKey(data.Id) ? MemoryStoreDateTimes[data.Id] : DateTime.MinValue;
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
            if (MemoryStore.ContainsKey(data.Id))
            {
                return MemoryStore[data.Id].ToArray();
            }

            return new ExecutableActionSpecification[0];
        }

        public void CreateActionEvent(T data)
        {
            if (!MemoryStore.ContainsKey(data.Id))
            {
                MemoryStore.Add(data.Id, new List<ExecutableActionSpecification>());
                MemoryStoreDateTimes.Add(data.Id, DateTime.UtcNow);
            }
        }

        public void AppendActionEvent(T data, ExecutableActionSpecification executableActionSpecification)
        {
            if (MemoryStore.ContainsKey(data.Id))
            {
                MemoryStore[data.Id].Add(executableActionSpecification);
                MemoryStoreDateTimes[data.Id] = DateTime.UtcNow;
            }
        }

        public void DeleteActionEvent(T data)
        {
            if (MemoryStore.ContainsKey(data.Id))
            {
                MemoryStore.Remove(data.Id);
                MemoryStoreDateTimes.Remove(data.Id);
            }
        }
    }
}
