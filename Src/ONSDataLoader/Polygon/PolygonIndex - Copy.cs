using Nest;
using NetTopologySuite.Geometries;
using NetTopologySuite.Index.Quadtree;
using ONSLoader.Console.Model;
using System.Collections.Generic;
using System.Linq;

namespace ONSLoader.Console.Polygon
{
    public partial class OldPolygonIndex
    {
        public Quadtree<GeometryData> Index=new Quadtree<GeometryData>();

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

            var all2 = items.Where(x => x.Geom.Contains(p)).ToList();
            return all2;
        }

        /// <summary>
        ///     returns a list of parent containers
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        public List<GeometryData> ContainedWithin(Geometry shape)
        {
            var envelope = shape.EnvelopeInternal;
            var items = Index.Query(envelope);
            var all2 = items.Where(x => x.Geom.Contains(shape)).ToList();
            return all2;
        }

        internal void AddRange(IEnumerable<GeometryData> sourceData)
        {
            foreach(var data in sourceData)
                Index.Insert(data.Geom.EnvelopeInternal, data);
        }
    }
}
