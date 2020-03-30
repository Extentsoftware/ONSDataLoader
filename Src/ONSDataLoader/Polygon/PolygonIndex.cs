using Nest;
using NetTopologySuite.Algorithm.Locate;
using NetTopologySuite.Geometries;
using NetTopologySuite.Index.Quadtree;
using ONSLoader.Console.Model;
using System.Collections.Generic;
using System.Linq;

namespace ONSLoader.Console.Polygon
{
    
    public class PolyLocator
    {
        public IndexedPointInAreaLocator Locator;
        public GeometryData Data;
    }
    public partial class PolygonIndex
    {
        public Quadtree<PolyLocator> Index=new Quadtree<PolyLocator>();

        public List<GeometryData> Search(GeoLocation coord)
        {
            return Search(coord.Longitude, coord.Latitude);
        }

        /// <summary>
        ///     find a list of polygons that contain the given point
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        public List<GeometryData> Search(double X, double Y, int srid = 4326)
        {
            var coord = new Coordinate(X, Y);
            var p = new Point(coord)
            {
                SRID = srid
            };

            var items = Index.Query(p.EnvelopeInternal);

            var all2 = items.Where(x => x.Locator.Locate(coord) == Location.Interior)
                .Select(x=>x.Data)
                .ToList();

            return all2;
        }
        
        internal void AddRange(IEnumerable<GeometryData> sourceData)
        {
            foreach (var data in sourceData)
            {
                var item = new PolyLocator
                {
                    Locator = new IndexedPointInAreaLocator(data.Geom),
                    Data = data
                };

                Index.Insert(data.Geom.EnvelopeInternal, item);
            }
        }
    }
}
