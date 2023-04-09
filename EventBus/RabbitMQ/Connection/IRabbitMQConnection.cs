using RabbitMQ.Client;

namespace RegistrationService.EventBus.RabbitMQ.Connection
{
    public interface IRabbitMQConnection
    {
        public IConnection TryConnect();
    }
}