using RabbitMQ.Client;

namespace RegistrationService.RabbitMQ.Connection
{
    public interface IRabbitMQConnection
    {
        public IConnection TryConnect();
    }
}