namespace ONSLoader.Console.Model
{
    public class CrossReferenceEntry
    {
        public int? Child { get; set; }
        public int? ChildRef { get; set; }
        public string ChildDataType { get; set; }
        public int? Parent { get; set; }
        public int? ParentRef { get; set; }
        public string ParentDataType { get; set; }
    }
}
