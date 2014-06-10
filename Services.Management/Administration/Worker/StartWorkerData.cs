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
        public string ContextType;
        public object[] Parameters;
        public string AdminHost;
        public int AdminPort;
        // public string AdminProtocol
        // public string AdminHttpPath
        public string ContextServerHost;
        public int ContextServerPort;
        // public string ContextProtocol
        // public string ContextHttpPath
        public List<ModuleStartEntry> Modules;
        public int ReportUpdateIntervalInSeconds = 5;

        public override int DataStructureVersionNumber
        {
            get
            {
                return 5;
            }
        }
    }
}
