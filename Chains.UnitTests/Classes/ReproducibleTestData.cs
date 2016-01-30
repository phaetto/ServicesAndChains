﻿namespace Chains.UnitTests.Classes
{
    using System;
    using Chains.Play;

    [Serializable]
    public class ReproducibleTestData : SerializableSpecification
    {
        public string DomainName;
        public string ChangeToValue;
        public string[] StringArray;

        public override int DataStructureVersionNumber => 1;
    }
}
