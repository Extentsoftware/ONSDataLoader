using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ONSLoader.Console.Elastic;
using ONSLoader.Console.Model;
using ONSLoader.Console.Patch;
using ONSLoader.Console.Pnt;
using ONSLoader.Console.Polygon;
using ONSLoader.Console.XRef;
using System;
using System.IO;
using System.Text;

namespace ONSLoader.Console
{
    /// <summary>
    /// https://geoportal.statistics.gov.uk/datasets/nuts-level-1-january-2018-generalised-clipped-boundaries-in-the-united-kingdom
    /// https://geoportal.statistics.gov.uk/datasets/nuts-level-2-january-2018-generalised-clipped-boundaries-in-the-united-kingdom
    /// https://geoportal.statistics.gov.uk/datasets/nuts-level-3-january-2018-generalised-clipped-boundaries-in-the-united-kingdom
    /// https://geoportal.statistics.gov.uk/datasets/local-administrative-units-level-1-january-2018-generalised-clipped-boundaries-in-united-kingdom
    /// https://geoportal.statistics.gov.uk/datasets/local-administrative-units-level-2-december-2015-generalised-clipped-boundaries-in-england-and-wales
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var config = GetConfiguration(args);

            var serviceProvider = ConfigureServices(config);

            var generator = serviceProvider.GetService<IndexGenerator>();
            var indexer = serviceProvider.GetService<ElasticIndexer>();

            var items = generator.Load();
            indexer.BulkIndex(items);

        }

        internal static IConfiguration GetConfiguration(string[] args)
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .AddJsonFile("appsettings.json", false);

            return  configBuilder.Build();
        }

        internal static IServiceProvider ConfigureServices(IConfiguration config)
        {
            var services = new ServiceCollection();

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConfiguration(config.GetSection("Logging"));
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
            });

            services.AddSingleton(config);
            var indexconfig = new IndexerConfig();
            
            config.GetSection(nameof(IndexerConfig)).Bind(indexconfig);
            services.AddSingleton(indexconfig);

            var elasticSettings = new ElasticSettings();
            config.GetSection(nameof(ElasticSettings)).Bind(elasticSettings);
            services.AddSingleton(elasticSettings);

            services.AddSingleton<IndexGenerator>();
            services.AddSingleton<CrossReferenceLoader>();
            services.AddSingleton<PointLoader>();
            services.AddSingleton<PolygonLoader>();
            services.AddSingleton<PatchLoader>();            
            services.AddSingleton<ElasticIndexer>();

            var builder = new ContainerBuilder();

            builder.Populate(services);

            var applicationContainer = builder.Build();

            var provider = new AutofacServiceProvider(applicationContainer);

            return provider;

        }

    }

}
