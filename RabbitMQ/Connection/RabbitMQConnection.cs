using RabbitMQ.Client;

namespace RegistrationService.RabbitMQ.Connection
{
    public class RabbitMQConnection : IRabbitMQConnection
    {
        private readonly IConnectionFactory factory;

        public RabbitMQConnection(string host, string username, string password)
        {
            factory = Uri.IsWellFormedUriString(host, UriKind.RelativeOrAbsolute)
                ? new ConnectionFactory
                {
                    Uri = new Uri(host),
                    UserName = username,
                    Password = password
                }
                : new ConnectionFactory
                {
                    HostName = host,
                    UserName = username,
                    Password = password
                };
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
