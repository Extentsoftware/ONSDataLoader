using ONSLoader.Console.Csv;

namespace ONSLoader.Console.Model
{
    public class PointFile
    {
        public string Filename { get; set; }
        public string Title { get; set; }
        public string DataType { get; set; }
        public int Name { get; set; }
        public int? MaxRecords { get; set; }
        public int? Reference { get; set; }        
        public int Latitude { get; set; }
        public int Longitude { get; set; }
        public int RowsToSkip { get; set; }
        public HeaderMode HeaderMode { get; set; }
        public string Separator { get; set; }
        public string Srid { get; set; }
    }
}
