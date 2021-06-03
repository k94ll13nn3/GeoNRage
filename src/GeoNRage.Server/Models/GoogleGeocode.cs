using System.Collections.Generic;

namespace GeoNRage.Server.GeoGuessr
{
    public class GoogleGeocode
    {
        public IList<GoogleGeocodeResult> Results { get; set; } = new List<GoogleGeocodeResult>();
    }
}
