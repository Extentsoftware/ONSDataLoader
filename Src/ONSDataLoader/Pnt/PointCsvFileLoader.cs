using NetTopologySuite.Geometries;
using ONSLoader.Console.Csv;
using ONSLoader.Console.Extensions;
using ONSLoader.Console.Model;
using ProjNet.CoordinateSystems;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ONSLoader.Console.Pnt
{
    public static class PointCsvFileLoader
    {
        public static IEnumerable<GeometryData> Load(PointFile source)
        {
            var fact = new GeometryFactory();
            using StreamReader reader = File.OpenText(source.Filename);

            var options = new CsvOptions { 
                HeaderMode = source.HeaderMode, 
                RowsToSkip = source.RowsToSkip, 
                Separator = source.Separator[0]  
            };

            CoordinateSystem f = ProjectionUtils.Transforms[source.Srid];
            CoordinateSystem t = ProjectionUtils.EPSG_4326();

            foreach (var row in CsvReader.Read(reader, options).Take(source.MaxRecords??int.MaxValue))
            {
                double.TryParse(row[source.Latitude], out double latitude);
                double.TryParse(row[source.Longitude], out double longitude);
                var geom = new Coordinate(longitude, latitude);
                
                geom = geom.Transform(f, t);

                var data = new GeometryData
                {
                    DataType = source.DataType,
                    Geom = fact.CreatePoint(geom),
                    Name = row[source.Name],
                    Reference = source.Reference != null ? row[(int)source.Reference] : null,
                };
                yield return data;
            }
        }
    }
}
