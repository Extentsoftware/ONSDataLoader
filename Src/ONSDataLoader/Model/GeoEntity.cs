using Nest;
using System.Collections.Generic;

namespace ONSLoader.Console.Model
{
    public class GeoEntity
    {
        public string EntityName { get; set; }
        public GeoEntityId GeoId { get; set; }
        public GeoLocation Location { get; set; }
        public PolygonGeoShape Polygon { get; set; }
        public List<GeoEntityId> Parents { get; set; }
    }
}
