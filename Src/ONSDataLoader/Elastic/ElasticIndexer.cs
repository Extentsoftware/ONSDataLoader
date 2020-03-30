using Microsoft.Extensions.Logging;
using Nest;
using ONSLoader.Console.Extensions;
using ONSLoader.Console.Model;
using System.Collections.Generic;
using System.Linq;

namespace ONSLoader.Console.Elastic
{
    public class ElasticIndexer
    {
        private readonly ElasticSettings _elasticSettings;
        private readonly ILogger<ElasticIndexer> _logger;

        public ElasticIndexer(
            ILogger<ElasticIndexer> logger,
            ElasticSettings elasticSettings)
        {
            _elasticSettings = elasticSettings;
            _logger = logger;
        }

        public int BulkIndex(IEnumerable<GeoEntity> entities)
        {
            var client = ElasticClientFactory.CreateClient(_elasticSettings);
            
            CreateIndex(client);

            var errorCount = 0;
            var batch = 0;

            foreach (var block in entities.Batch(_elasticSettings.BatchSize))
            {
                var bulkRequest = new BulkRequest()
                {
                    Operations = new List<IBulkOperation>()
                };

                foreach (var item in block)
                    bulkRequest.Operations.Add(new BulkIndexOperation<GeoEntity>(item));

                batch++;
                var result = client.Bulk(bulkRequest);
                var errors = result.ItemsWithErrors.Count();

                if (errors > 0)
                    _logger.LogWarning("{}");

                errorCount += errors;
            }

            return errorCount;
        }

        void CreateIndex(ElasticClient client)
        {
            if (client.Indices.Exists(_elasticSettings.DefaultIndex).Exists)
                client.Indices.Delete(_elasticSettings.DefaultIndex);

            var result = client.Indices.Create(_elasticSettings.DefaultIndex, s => s
                    .Settings(settings => settings
                    .NumberOfShards(_elasticSettings.Shards)
                    .NumberOfReplicas(_elasticSettings.Replicas))
                    .Map<GeoEntity>(m => m
                        .AutoMap()
                        .Properties(p => p
                            .Keyword(str => str.Name(xg => xg.EntityName))
                            .GeoPoint(g => g.Name(xg => xg.Location))
                            .GeoShape(g => g.Name(xg => xg.Polygon))
                        )
                    )
                    .Map<GeoEntityId>(m => m
                        .AutoMap()
                        .Properties(p => p
                            .Keyword(str => str.Name(xg => xg.Name))
                            .Keyword(str => str.Name(xg => xg.Reference))
                            .Keyword(str => str.Name(xg => xg.DataType))
                        )
                    ));
        }
    }
}
