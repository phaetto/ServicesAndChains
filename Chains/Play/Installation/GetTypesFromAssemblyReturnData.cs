﻿namespace Chains.Play.Installation
{
    using System;

    [Serializable]
    public class GetTypesFromAssemblyReturnData : SerializableSpecification
    {
        public string[] Types;

        public override int DataStructureVersionNumber => 1;
    }
}
