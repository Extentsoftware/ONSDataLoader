using Nest;
using NetTopologySuite.Geometries;
using ProjNet.CoordinateSystems.Transformations;
using System;
using System.Collections.Generic;

namespace ONSLoader.Console.Extensions
{
    public static class GeomUtils
    {
        public static PolygonGeoShape GetPolygon(Geometry geom, ICoordinateTransformation transformer=null)
        {
            try
            {
                var listofpoints = new List<List<GeoCoordinate>>();

                // only take the first (outer) polygon
                var subgeom = geom.GetGeometryN(0);
                var numpoints = subgeom.NumPoints;
                var points = new List<GeoCoordinate>();

                for (var i = 0; i < numpoints; i++)
                {
                    var coord = subgeom.Coordinates[i];
                    if (transformer != null)
                        coord = coord.Transform(transformer);
                    points.Add(new GeoCoordinate(coord.Y, coord.X));
                }

                var close = points[0].Longitude != points[numpoints - 1].Longitude ||
                            points[0].Latitude != points[numpoints - 1].Latitude
                            ? 1 : 0;

                // add 1st point as last to complete the polygon
                if (close == 1)
                {
                    points.Add(new GeoCoordinate(points[0].Latitude, points[0].Longitude));
                }

                listofpoints.Add(points);

                PolygonGeoShape result = new PolygonGeoShape(listofpoints);
                return result;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error", ex);
            }
        }
    }
}
