using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using ONSLoader.Console.Extensions;
using ONSLoader.Console.Model;
using ProjNet.CoordinateSystems;
using System;
using System.Collections.Generic;

namespace ONSLoader.Console.Polygon
{
    public static class ShapefileLoader
    {
        public static IEnumerable<GeometryData> Load(PolygonFile source)
        {
            var geomFact = new GeometryFactory();
            using var reader = new ShapefileDataReader(source.Filename, geomFact);

            CoordinateSystem f = ProjectionUtils.Transforms[source.Srid];
            CoordinateSystem t = ProjectionUtils.EPSG_4326();
            var transformer = ProjectionUtils.GetTransformer(f, t);

            while (reader.Read())
            {
                var name = reader.GetString(source.Name);
                string reference = null;
                
                if (source.Reference != null)
                    reference = reader.GetString((int)source.Reference);

                if (reference == null)
                    reference = Guid.NewGuid().ToString();
                
                var geom = ProjectionUtils.Transform(reader.Geometry, transformer);
                
                var data = new GeometryData { Name = name, DataType = source.DataType, Reference = reference, Geom = geom };

                yield return data;
            }
        }
    }
}
