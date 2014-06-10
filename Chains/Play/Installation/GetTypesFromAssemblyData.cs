﻿namespace Chains.Play.Installation
{
    using System;

    [Serializable]
    public class GetTypesFromAssemblyData : SerializableSpecification
    {
        public string FileName;

        public override int DataStructureVersionNumber
        {
            get
            {
                return 1;
            }
        }
    }
}
