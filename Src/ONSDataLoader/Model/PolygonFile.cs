namespace ONSLoader.Console.Model
{
    public class PolygonFile
    {
        public string Filename { get; set; }
        public string Title { get; set; }
        public string Processor { get; set; }
        public int Name { get; set; }
        public int? Parent { get; set; }
        public int? Reference { get; set; }
        public string Srid { get; set; }
        public string DataType { get; set; }
    }

}
