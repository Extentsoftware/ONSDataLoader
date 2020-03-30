using ONSLoader.Console.Csv;
using ONSLoader.Console.Model;
using System.Collections.Generic;
using System.IO;

namespace ONSLoader.Console.Patch
{
    public static class CsvPatchFileLoader
    {
        public static IEnumerable<PatchItem> Load(PatchFile source)
        {
            using StreamReader reader = File.OpenText(source.Filename);

            var options = new CsvOptions { HeaderMode = source.HeaderMode, RowsToSkip = source.RowsToSkip, Separator = source.Separator[0] };

            foreach (var row in CsvReader.Read(reader, options))
            {
                var data = new PatchItem
                {
                    From = new GeoEntityId { DataType = row[0], Reference = row[1], Name = row[2] },
                    To = new GeoEntityId { DataType = row[3], Reference = row[4], Name = row[5] }
                };
                yield return data;
            }            
        }
    }
}
