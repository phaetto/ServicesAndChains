namespace Services.Geolocation
{
    using Chains;

    public sealed class LocateContext : Chain<LocateContext>
    {
        public readonly bool UseWifi;

        public LocateContext(bool useWifi = true)
        {
            UseWifi = useWifi;
        }
    }
}
