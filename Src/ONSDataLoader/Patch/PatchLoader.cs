using ONSLoader.Console.Model;
using System.Collections.Generic;

namespace ONSLoader.Console.Patch
{
    public class PatchLoader
    {
        public IEnumerable<GeoEntity> Load(List<PatchFile> sourceFiles, List<CrossReference> crossreferences)
        {
            List<PatchItem> patches = new List<PatchItem>();
            foreach (var sourcefile in sourceFiles)
            {
                var items = CsvPatchFileLoader.Load(sourcefile);
                if (items != null)
                    patches.AddRange(items);
            }
            return Patch(patches, crossreferences);
        }

        private IEnumerable<GeoEntity> Patch(List<PatchItem> patches, List<CrossReference> crossreferences)
        {
            foreach (var patch in patches)
            {
                var items = Patch(patch, crossreferences);
                foreach(var item in items)
                    yield return item;
            }                
        }

        private IEnumerable<GeoEntity> Patch(PatchItem patch, List<CrossReference> crossreferences)
        {
            return new List<GeoEntity>();
        }

    }
}
