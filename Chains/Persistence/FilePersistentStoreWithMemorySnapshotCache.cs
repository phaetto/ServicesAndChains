namespace Chains.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Chains.Play;

    public class FilePersistentStoreWithMemorySnapshotCache<T> : IPersistentStore<T>
        where T : SerializableSpecificationWithId, new()
    {
        private static Dictionary<Guid, T> snapshotMemoryStore = new Dictionary<Guid, T>();
        private static Dictionary<Guid, DateTime> snapshotsStoreDateTimes = new Dictionary<Guid, DateTime>();

        public readonly string folder;
        public readonly string directory;

        public FilePersistentStoreWithMemorySnapshotCache(string folder)
        {
            this.folder = folder;
            directory = folder + Path.DirectorySeparatorChar + typeof(T).FullName;

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        private string GetFilePath(T data)
        {
            return directory + Path.DirectorySeparatorChar + data.Id + ".jsonlist";
        }

        private string GetSnapshotFilePath(T data)
        {
            return directory + Path.DirectorySeparatorChar + data.Id + ".json";
        }

        public bool SnapshotExists(T data)
        {
            return snapshotMemoryStore.ContainsKey(data.Id) || File.Exists(GetSnapshotFilePath(data));
        }

        public DateTime GetLastActionEventTime(T data)
        {
            return File.Exists(GetFilePath(data)) ? File.GetLastWriteTimeUtc(GetFilePath(data)) : DateTime.MinValue;
        }

        public T LoadSnapshot(T data)
        {
            // Check memory
            if (snapshotMemoryStore.ContainsKey(data.Id)
                && GetLastActionEventTime(data) <= snapshotsStoreDateTimes[data.Id])
            {
                return snapshotMemoryStore[data.Id];
            }

            // Check file
            if (File.Exists(GetSnapshotFilePath(data))
                && GetLastActionEventTime(data) <= File.GetLastWriteTimeUtc(GetSnapshotFilePath(data)))
            {
                var json = File.ReadAllText(GetSnapshotFilePath(data));

                return DeserializableSpecification<T>.DeserializeFromJson(json);
            }

            return null;
        }

        public void SaveSnapshot(T data)
        {
            if (snapshotMemoryStore.ContainsKey(data.Id))
            {
                snapshotsStoreDateTimes[data.Id] = DateTime.UtcNow;
                snapshotMemoryStore[data.Id] = data;
            }
            else
            {
                snapshotsStoreDateTimes.Add(data.Id, DateTime.UtcNow);
                snapshotMemoryStore.Add(data.Id, data);
            }

            // Should be done async
            File.WriteAllText(GetSnapshotFilePath(data), data.SerializeToJson());
        }

        public ExecutableActionSpecification[] LoadActionEvents(T data)
        {
            if (File.Exists(GetFilePath(data)))
            {
                var lines = File.ReadAllLines(GetFilePath(data));
                var executableSpecifications =
                    lines.Where(x => !string.IsNullOrWhiteSpace(x))
                         .Select(DeserializableSpecification<ExecutableActionSpecification>.DeserializeFromJson)
                         .ToArray();

                return executableSpecifications;
            }

            return new ExecutableActionSpecification[0];
        }

        public void CreateActionEvent(T data)
        {
            File.WriteAllText(GetFilePath(data), string.Empty);
        }

        public void AppendActionEvent(T data, ExecutableActionSpecification executableActionSpecification)
        {
            if (File.Exists(GetFilePath(data)))
            {
                File.AppendAllText(GetFilePath(data), executableActionSpecification.SerializeToJson() + Environment.NewLine);
            }
        }

        public void DeleteActionEvent(T data)
        {
            if (File.Exists(GetFilePath(data)))
            {
                File.Delete(GetFilePath(data));
            }

            if (File.Exists(GetSnapshotFilePath(data)))
            {
                File.Delete(GetSnapshotFilePath(data));
            }

            snapshotMemoryStore.Remove(data.Id);
        }
    }
}
