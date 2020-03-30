using Microsoft.Extensions.Logging;
using ONSLoader.Console.Model;
using System.Collections.Generic;
using System.Linq;

namespace ONSLoader.Console.Pnt
{
    public class PointLoader
    {
        private readonly ILogger<PointLoader> _logger;

        public PointLoader(ILogger<PointLoader> logger)
        {
            _logger = logger;
        }

        public List<GeometryData> Load(List<PointFile> sourceFiles)
        {
            List<GeometryData> sourceData = new List<GeometryData>();
            foreach (var sourcefile in sourceFiles)
            {
                var items = ProcessSourceFile(sourcefile);
                if (items != null)
                    sourceData.AddRange(items);
            }

            return sourceData;
        }

        IEnumerable<GeometryData> ProcessSourceFile(PointFile sourcefile)
        {
            var items = PointCsvFileLoader.Load(sourcefile).ToList();
            _logger.LogInformation($"Loaded {items.Count} from {sourcefile.Title}");
            return items;
        }
    }
}
