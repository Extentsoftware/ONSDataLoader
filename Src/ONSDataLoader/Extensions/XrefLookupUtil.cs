using ONSLoader.Console.Model;
using System.Collections.Generic;
using System.Linq;

namespace ONSLoader.Console.Extensions
{
    public static class XrefLookupUtil
    {
        public static void LookupParentsByXRef(List<GeoEntity> data, List<CrossReference> crossreferences)
            => data.AsParallel()
                    .WithMergeOptions(ParallelMergeOptions.AutoBuffered)
                    .ForAll(x => AddParents(x, data, crossreferences));

        public static GeoEntity AddParents(GeoEntity entity, IEnumerable<GeoEntity> sourceData, List<CrossReference> crossreferences)
        {
            entity.Parents = FindParents(entity.GeoId, sourceData, crossreferences);
            return entity;
        }

        public static List<GeoEntityId> FindParents(GeoEntityId id, IEnumerable<GeoEntity> sourceData, List<CrossReference> crossreferences)
        {
            List<GeoEntityId> allparents = new List<GeoEntityId>();

            var parents = crossreferences.Where(x => id.Reference == x.Child.Reference && x.Child.DataType == id.DataType)
                .Select(x => x.Parent)
                .Distinct()
                .ToList();

            allparents.AddRange(parents);

            foreach (var parent in parents)
            {
                var moreParents = FindParents(parent, sourceData, crossreferences);
                allparents.AddRange(moreParents);
            }

            return allparents;
        }

    }
}
