using Nest;
using NetTopologySuite.Geometries;

namespace ONSLoader.Console.Extensions
{
    public static class ElasticGeometryUtil
    {
        public static PolygonGeoShape GetPolygonGeoShape(Geometry geom)
        {
            return geom.GeometryType switch
            {
                "MultiPolygon" => GeomUtils.GetPolygon(geom.Boundary),
                "Polygon" => GeomUtils.GetPolygon(geom),
                _ => null,
            };
        }

        public static GeoLocation GetGeoLocation(Geometry geom)
        {
            return geom.GeometryType switch
            {
                "Point" => new GeoLocation(geom.Coordinate.Y, geom.Coordinate.X),
                _ => null,
            };
        }
    }
}
