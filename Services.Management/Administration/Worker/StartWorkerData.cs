namespace Services.Management.Administration.Worker
{
    using System;
    using System.Collections.Generic;
    using Chains.Play;

    [Serializable]
    public class StartWorkerData : SerializableSpecification
    {
        public string Id;
        public string ServiceName;
        public int Version;
        public string HostProcessFileName;
        public string DllPath;
        public string FilesRepository;
        public string ContextType;
        public object[] Parameters;
        public string AdminHost;
        public int AdminPort;
        public string ContextServerHost;
        public int ContextServerPort;
        public int ContextServerThreads;
        public List<ModuleStartEntry> Modules;
        public int ReportUpdateIntervalInSeconds = 5;
        public StartWorkerHttpData ContextHttpData;

        public override int DataStructureVersionNumber => 5;
    }
}
