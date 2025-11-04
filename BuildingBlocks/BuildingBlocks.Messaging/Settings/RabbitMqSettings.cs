namespace BuildingBlocks.Messaging.Settings
{
    public class RabbitMqSettings
    {
        public string Host { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int Port { get; set; } = 5672;
        public string VirtualHost { get; set; } = "/";
        public string ClientProvidedName { get; set; } = "LifeSync";
    }
}
