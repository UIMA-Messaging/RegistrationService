using RabbitMQ.Client;

namespace RegistrationApi.EventBus.RabbitMQ.Connection
{
    public interface IRabbitMQConnection
    {
        public IConnection TryConnect();
    }
}