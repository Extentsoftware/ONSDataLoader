using NetTopologySuite.Geometries;
using ONSLoader.Console.Model;
using System.Collections.Generic;
using System.Linq;

namespace ONSLoader.Console
{
    public static class OutcodeGenerator
    {
        public static IEnumerable<GeometryData> GenerateOutcodes(IEnumerable<GeometryData> data)
        {
            var outcodes = data.GroupBy(x => Outcode(x.Name));
            var fact = new GeometryFactory();
            foreach (var outcode in outcodes.Where(x=>x.Key=="BR6"))
            {
                var coords = outcode.Where(x=>!(x.Geom.Coordinate.X ==0 && x.Geom.Coordinate.Y== 99.999999))
                    .Select(x => x.Geom.Coordinate).ToArray();

                var coord = new Coordinate(coords.Average(x => x.X), coords.Average(x => x.Y));

                yield return new GeometryData
                {
                    DataType = "Outcode",
                    Name = outcode.Key,
                    Reference = outcode.Key,
                    Geom = fact.CreatePoint(coord)
                };
            }
        }

        private static string Outcode(string postcode)
            => postcode.Substring(0, postcode.IndexOf(' '));
    }
}
