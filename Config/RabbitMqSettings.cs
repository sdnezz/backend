using RabbitMQ.Client;

namespace SolutionLab1.Config;

public class RabbitMqSettings
{
    public string HostName { get; set; }

    public int Port { get; set; }
    
    public string OrderCreatedQueue { get; set; }
}