using EmailSender.Infrastructure.Messaging;

namespace EmailSender.API
{
    public class Worker : BackgroundService
    {
        private readonly RabbitMqEventConsumer _rabbitMqEventConsumer;

        public Worker(RabbitMqEventConsumer rabbitMqEventConsumer)
        {
            _rabbitMqEventConsumer = rabbitMqEventConsumer;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _rabbitMqEventConsumer.StartConsuming();

            return Task.Delay(-1, stoppingToken);
        }
    }
}
