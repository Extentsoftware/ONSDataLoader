using ONSLoader.Console.Model;
using System.Collections.Generic;
using System.Linq;

namespace ONSLoader.Console.XRef
{
    public class CrossReferenceLoader
    {
        public List<CrossReference> Load(List<CrossReferenceFile> sourceFiles)
        {
            List<CrossReference> sourceData = new List<CrossReference>();
            foreach (var sourcefile in sourceFiles)
            {
                var items = ProcessSourceFile(sourcefile);
                if (items != null)
                    sourceData.AddRange(items);
            }
            return sourceData;
        }

        IEnumerable<CrossReference> ProcessSourceFile(CrossReferenceFile sourcefile)
        {
            switch (sourcefile.Processor)
            {
                case "csvfile":
                    var items = CsvCrossReferencefileLoader.Load(sourcefile).ToList();
                    return items;
            }
            return null;
        }
    }
}
