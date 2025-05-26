using RabbitMQ.Client;
using System.Collections.Generic;

namespace BuildingBlocks.Messaging.Options
{
    public class PublishOptions
    {
        public string ExchangeName { get; set; } = "default.exchange";
        public string TypeExchange { get; set; } = ExchangeType.Fanout;
        public string RoutingKey { get; set; } = "";
        public bool Durable { get; set; } = true;
        public bool AutoDelete { get; set; } = false;
        public IDictionary<string, object>? Arguments { get; set; } = null;
    }
}