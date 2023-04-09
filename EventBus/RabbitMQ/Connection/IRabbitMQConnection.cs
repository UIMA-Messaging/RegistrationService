using RabbitMQ.Client;

namespace UserService.EventBus.RabbitMQ.Connection
{
    public interface IRabbitMQConnection
    {
        public IConnection TryConnect();
    }
}