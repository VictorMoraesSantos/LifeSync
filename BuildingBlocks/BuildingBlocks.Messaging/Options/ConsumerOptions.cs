using RabbitMQ.Client;

namespace BuildingBlocks.Messaging.Options
{
    public class ConsumerOptions
    {
        public string QueueName { get; set; } = "";
        public string ExchangeName { get; set; } = "";
        public string TypeExchange { get; set; } = ExchangeType.Fanout;
        public string RoutingKey { get; set; } = "";
        public bool Durable { get; set; } = true;
        public bool AutoDelete { get; set; } = false;
        public IDictionary<string, object>? Arguments { get; set; }
    }
}
