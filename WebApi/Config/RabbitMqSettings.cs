using RabbitMQ.Client;

namespace WebApi.Config;

public class RabbitMqSettings
{
    public string HostName { get; set; }
    public int Port { get; set; }
    public string Exchange { get; set; }
    public ExchangeMapping[] ExchangeMappings { get; set; }
    public class ExchangeMapping
    {
        public string Queue { get; set; }
        
        public string RoutingKeyPattern { get; set; }
    }
    public string OrderCreatedQueue { get; set; }
    public ushort BatchSize { get; set; }
    public int BatchTimeoutSeconds { get; set; }
}