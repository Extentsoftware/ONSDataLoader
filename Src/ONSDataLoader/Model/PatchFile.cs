using ONSLoader.Console.Csv;

namespace ONSLoader.Console.Model
{
    public class PatchFile
    {
        public string Filename { get; set; }
        public string Title { get; set; }
        public int RowsToSkip { get; set; }
        public string Separator { get; set; }
        public HeaderMode HeaderMode { get; set; }
    }
}
