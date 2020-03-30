using ONSLoader.Console.Csv;
using ONSLoader.Console.Model;
using System.Collections.Generic;
using System.IO;

namespace ONSLoader.Console.XRef
{

    public static class CsvCrossReferencefileLoader
    {
        public static IEnumerable<CrossReference> Load(CrossReferenceFile source)
        {
            using StreamReader reader = File.OpenText(source.Filename);

            var options = new CsvOptions { HeaderMode = source.HeaderMode, RowsToSkip = source.RowsToSkip, Separator = source.Separator[0] };

            foreach (var row in CsvReader.Read(reader, options))
            {
                foreach( var xref in source.Items)
                {
                    var data = new CrossReference
                    {
                        Child = new GeoEntityId
                        {
                            Name = xref.Child != null ? row[(int)xref.Child] : null,
                            Reference = xref.ChildRef != null ? row[(int)xref.ChildRef] : null,
                            DataType = xref.ChildDataType
                        },
                        Parent = new GeoEntityId
                        {
                            Name = xref.Parent != null ? row[(int)xref.Parent] : null,
                            Reference = xref.ParentRef != null ? row[(int)xref.ParentRef] : null,
                            DataType = xref.ParentDataType
                        },
                    };
                    yield return data;
                }
            }
        }
    }
}
