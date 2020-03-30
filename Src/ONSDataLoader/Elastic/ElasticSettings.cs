using System.Collections.Generic;

namespace ONSLoader.Console.Elastic
{
    public class ElasticSettings
    {
        public string User { get; set; }
        public string Password { get; set; }
        public string ProxyServer { get; set; }
        public string ProxyUser { get; set; }
        public string ProxyPassword { get; set; }
        public string DefaultIndex { get; set; }
        public List<string> ElasticUrls { get; set; }
        public int BatchSize { get; set; } = 1000;
        public int Shards { get; set; } = 1;
        public int Replicas { get; set; } = 0;
        

    }
}
