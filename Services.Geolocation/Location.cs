namespace Services.Geolocation
{
    using System;
    using Chains.Play;

    [Serializable]
    public sealed class Location : SerializableSpecification
    {
        public override int DataStructureVersionNumber
        {
            get
            {
                return 1;
            }
        }

        public double Longitude;

        public double Latitude;

        public double Accuracy;
    }
}
