using Microsoft.Extensions.Logging;
using ONSLoader.Console.Extensions;
using ONSLoader.Console.Model;
using ONSLoader.Console.Pnt;
using ONSLoader.Console.Polygon;
using ONSLoader.Console.XRef;
using System.Collections.Generic;
using System.Linq;

namespace ONSLoader.Console
{
    public class IndexGenerator
    {
        private readonly ILogger<IndexGenerator> _logger;
        private readonly PolygonLoader _polygonLoader;
        private readonly CrossReferenceLoader _crossReferenceLoader;
        private readonly PointLoader _pointLoader;
        private readonly IndexerConfig _config;

        public IndexGenerator(
            ILogger<IndexGenerator> logger,
            IndexerConfig config,
            PolygonLoader sourceLoader,
            PointLoader pointLoader,
            CrossReferenceLoader crossReferenceLoader )
        {
            _logger = logger;
            _polygonLoader = sourceLoader;
            _crossReferenceLoader = crossReferenceLoader;
            _pointLoader = pointLoader;
            _config = config;
        }

        public IEnumerable<GeoEntity> Load()
        {
            var (admindocs, adminGeometries) = LoadAdminAreasData();

            var (postcodeDocs, outcodeDocs) = LoadPostcodeData(adminGeometries);

            foreach (var entity in admindocs)
                yield return entity;

            foreach (var entity in outcodeDocs)
                yield return entity;

            foreach (var entity in postcodeDocs)
                yield return entity;
        }

        private (IEnumerable<GeoEntity> admindocs, List<GeometryData> adminGeometries) LoadAdminAreasData()
        {
            _logger.LogInformation($"Loading cross references");
            var crossreferences = _crossReferenceLoader.Load(_config.CrossReferenceFiles);
            _logger.LogInformation($"Loaded {crossreferences.Count} cross references");

            _logger.LogInformation($"Loading source records");
            var adminGeometries = _polygonLoader
                .Load(_config.PolygonFiles);

            _logger.LogInformation($"Loaded {adminGeometries.Count()} source records");

            // convert polydata into entities and lookup parents using xreferences
            var adminGeoEntities = FromGeometryData(adminGeometries).ToList();

            // move this up..
            _logger.LogInformation($"Finding Parents");
            XrefLookupUtil.LookupParentsByXRef(adminGeoEntities, crossreferences);

            return (adminGeoEntities, adminGeometries);
        }

        private (IEnumerable<GeoEntity> postcodes, IEnumerable<GeoEntity> outcodes) LoadPostcodeData(IEnumerable<GeometryData> sourceData)
        {
            _logger.LogInformation($"Loading points");
            var pointData = _pointLoader
                .Load(_config.PointFiles);
            var postcodeDocs = FromGeometryData(pointData);
            _logger.LogInformation($"Loaded {pointData.Count()} points");

            _logger.LogInformation($"Finding outcode parents using geometries");
            var outcodes = OutcodeGenerator.GenerateOutcodes(pointData);
            _logger.LogInformation($"Found {outcodes.Count()} Outcodes");

            var outcodeDocs = FromGeometryData(outcodes).ToList();

            _logger.LogInformation($"Finding Outcode parents using geometry");
            GeometrylookupUtil.LookupParentsByGeometry(outcodeDocs, sourceData);
            _logger.LogInformation($"Found parents for {outcodes.Count()} Outcodes");

            return (postcodeDocs, outcodeDocs);
        }

        private IEnumerable<GeoEntity> FromGeometryData(IEnumerable<GeometryData> data)
        {
            var entities = data
                    .Select(x => new GeoEntity
                    { 
                        EntityName = x.Name,
                        GeoId = new GeoEntityId
                        {
                            Reference = x.Reference,
                            Name = x.Name,
                            DataType = x.DataType
                        },
                        Polygon = ElasticGeometryUtil.GetPolygonGeoShape(x.Geom),
                        Location = ElasticGeometryUtil.GetGeoLocation(x.Geom),
                        Parents = new List<GeoEntityId>()
                    }).ToList(); 

            return entities;
        }
    }
}
