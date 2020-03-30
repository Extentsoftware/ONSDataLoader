using Microsoft.Extensions.Logging;
using ONSLoader.Console.Model;
using System.Collections.Generic;
using System.Linq;

namespace ONSLoader.Console.Polygon
{
    public class PolygonLoader
    {
        private readonly ILogger<PolygonLoader> _logger;

        public PolygonLoader(ILogger<PolygonLoader> logger)
        {
            _logger = logger;
        }

        public List<GeometryData> Load(List<PolygonFile> sourceFiles)
        {
            List<GeometryData> sourceData = new List<GeometryData>();
            foreach ( var sourcefile in sourceFiles)
            {
                var items = ProcessSourceFile(sourcefile);
                if (items != null)
                    sourceData.AddRange(items);
            }
            return sourceData;
        }

        IEnumerable<GeometryData> ProcessSourceFile(PolygonFile sourcefile)
        {
            switch (sourcefile.Processor)
            {
                case "shapefile":
                    {
                        var items = ShapefileLoader.Load(sourcefile).ToList();
                        _logger.LogInformation($"Loaded {items.Count} from {sourcefile.Title}");
                        return items;
                    }
            }
            return null;
        }
    }
}
