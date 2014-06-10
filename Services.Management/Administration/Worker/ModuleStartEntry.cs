namespace Services.Management.Administration.Worker
{
    using System;
    using System.Collections.Generic;
    using Chains.Play;

    [Serializable]
    public class ModuleStartEntry : SerializableSpecification
    {
        public string ModuleType;
        public object[] ModuleParameters;
        public string ModuleDllPath;
        public List<ModuleStartEntry> Modules;
        public bool IsEnabled = true;

        public override int DataStructureVersionNumber
        {
            get
            {
                return 1;
            }
        }
    }
}
