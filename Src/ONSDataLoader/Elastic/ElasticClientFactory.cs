using Elasticsearch.Net;
using Nest;
using System;
using System.Linq;

namespace ONSLoader.Console.Elastic
{
    public class ElasticClientFactory
    {
        public static ElasticClient CreateClient(ElasticSettings settings)
        {
            var urls = settings.ElasticUrls.Select(x => new Uri(x)).ToArray();
            var pool = new StaticConnectionPool(urls);
            var connsettings = new ConnectionSettings(pool);
            if (!string.IsNullOrEmpty(settings.User))
            {
                connsettings.BasicAuthentication(settings.User, settings.Password);
            }

            if (!string.IsNullOrEmpty(settings.ProxyServer))
            {
                connsettings.Proxy(new Uri(settings.ProxyServer), settings.ProxyUser, settings.ProxyPassword);
            }

            connsettings.DefaultIndex(settings.DefaultIndex);

            ElasticClient client = new ElasticClient(connsettings);

            return client;
        }
    }
}
