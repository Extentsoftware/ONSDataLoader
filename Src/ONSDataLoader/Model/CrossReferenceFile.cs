using ONSLoader.Console.Csv;
using System.Collections.Generic;

namespace ONSLoader.Console.Model
{
    public class CrossReferenceFile
    {
        public List<CrossReferenceEntry> Items { get; set; }
        public string Filename { get; set; }
        public string Title { get; set; }
        public string Processor { get; set; }
        public int? Reference { get; set; }
        public int RowsToSkip { get; set; }
        public string Separator { get; set; }
        public HeaderMode HeaderMode { get; set; }
    }
}
