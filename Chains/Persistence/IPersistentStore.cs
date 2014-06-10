namespace Chains.Persistence
{
    using System;
    using Chains.Play;

    public interface IPersistentStore<T>
        where T : SerializableSpecificationWithId
    {
        bool SnapshotExists(T data);

        DateTime GetLastActionEventTime(T data);

        ExecutableActionSpecification[] LoadActionEvents(T data);

        T LoadSnapshot(T data);

        void SaveSnapshot(T data);

        void CreateActionEvent(T data);

        void AppendActionEvent(T data, ExecutableActionSpecification executableActionSpecification);

        void DeleteActionEvent(T data);
    }
}