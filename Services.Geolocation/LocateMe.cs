namespace Services.Geolocation
{
    using System.Net;
    using System.Text;
    using Chains.Play;
    using NativeWifi;
    using Newtonsoft.Json;

    public sealed class LocateMe :  RemotableAction<Location, LocateContext>
    {
        public const string BaseUri =
            "https://maps.googleapis.com/maps/api/browserlocation/json?browser=&sensor=true";

        protected override Location ActRemotely(LocateContext context)
        {
            var url = BaseUri;

            if (context.UseWifi)
            {
                using (var client = new WlanClient())
                {
                    foreach (var wlanIface in client.Interfaces)
                    {
                        var wlanBssEntries = wlanIface.GetNetworkBssList();
                        foreach (var network in wlanBssEntries)
                        {
                            var macAddr = network.dot11Bssid;
                            var macId = string.Empty;
                            for (var i = 0; i < macAddr.Length; i++)
                            {
                                macId += macAddr[i].ToString("x2").PadLeft(2, '0').ToUpper();
                                macId += "-";
                            }

                            macId = macId.Trim(
                                new[]
                                {
                                    ':', '.', '-'
                                });

                            url += string.Format(
                                "&wifi=mac:{0}|ssid:{1}|ss:{2}",
                                macId,
                                GetStringForSSID(network.dot11Ssid),
                                network.linkQuality);
                        }
                    }
                }
            }

            using (var client = new WebClient())
            {
                var response = client.DownloadString(url);

                var responseObject = JsonConvert.DeserializeObject<GoogleLocationResponse>(response);

                return new Location
                       {
                           Longitude = responseObject.location.lng,
                           Latitude = responseObject.location.lat,
                           Accuracy = responseObject.accuracy
                       };
            }
        }

        static string GetStringForSSID(Wlan.Dot11Ssid ssid)
        {
            return Encoding.ASCII.GetString(ssid.SSID, 0, (int)ssid.SSIDLength);
        }
    }
}
