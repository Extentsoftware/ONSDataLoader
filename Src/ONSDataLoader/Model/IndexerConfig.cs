using System.Collections.Generic;

namespace ONSLoader.Console.Model
{
    public class IndexerConfig
    {
        public List<PointFile> PointFiles { get; set; }
        public List<CrossReferenceFile> CrossReferenceFiles { get; set; }
        public List<PolygonFile> PolygonFiles { get; set; }
        public List<PatchFile> PatchFiles { get; set; }        
    }
}
