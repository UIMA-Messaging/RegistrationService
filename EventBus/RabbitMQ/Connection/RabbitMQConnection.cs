using RabbitMQ.Client;

namespace UserService.EventBus.RabbitMQ.Connection
{
    public class RabbitMQConnection : IRabbitMQConnection
    {
        private readonly IConnectionFactory factory;

        public RabbitMQConnection(string host)
        {
            factory = new ConnectionFactory() { HostName = host };
        }

        public IConnection TryConnect()
        {
            const int retryCount = 5;
            var retries = 0;
            while (true)
            {
                try
                {
                    return factory.CreateConnection();
                }
                catch (Exception)
                {
                    retries++;
                    if (retries == retryCount) throw;
                    Thread.Sleep((int)Math.Pow(retries, 2) * (500 + new Random().Next(500)));
                }
            }
        }
    }
}
