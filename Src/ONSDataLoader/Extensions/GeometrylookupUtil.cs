using ONSLoader.Console.Model;
using ONSLoader.Console.Polygon;
using System.Collections.Generic;
using System.Linq;

namespace ONSLoader.Console.Extensions
{
    public static class GeometrylookupUtil
    {
        public static void LookupParentsByGeometry(List<GeoEntity> data, IEnumerable<GeometryData> crossreferences)
        {
            var polyIndex = new PolygonIndex();

            polyIndex.AddRange(crossreferences);

            data.ForEach(x => AddParents(x, polyIndex));

            //.AsParallel()
            //.WithMergeOptions(ParallelMergeOptions.AutoBuffered)
            //.ForAll(x => AddParents(x, polyIndex));
        }

        public static GeoEntity AddParents(GeoEntity entity, PolygonIndex index)
        {
            var entities = index.Search(entity.Location);

            entity.Parents = entities.Select(x=> new GeoEntityId
            {
                DataType = x.DataType,
                Name = x.Name,
                Reference = x.Reference
            }).ToList();

            return entity;
        }
    }
}
