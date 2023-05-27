using Newtonsoft.Json;
using RabbitMQ.Client;
using RegistrationService.RabbitMQ.Connection;
using System.Diagnostics;
using System.Text;

namespace RegistrationService.RabbitMQ
{
    public class RabbitMQPublisher<T> : IRabbitMQPublisher<T>
    {
        private readonly IModel channel;
        private readonly string exchange;

        public RabbitMQPublisher(IRabbitMQConnection connection, string exchange)
        {
            try
            {
                var client = connection.TryConnect();

                channel = client.CreateModel();
                this.exchange = exchange;

                channel.ExchangeDeclare(exchange, ExchangeType.Topic, durable: true);

                Debug.WriteLine($" [x] Ready to publish to exchange {exchange}");
            }
            catch { }
        }

        public void Publish(T message, params string[] routingKeys)
        {
            if (channel == null) return;

            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            foreach (var key in routingKeys)
            {
                channel.BasicPublish(exchange, key, properties, body);
            }

            Debug.WriteLine($" [x] Sent '{message}' to '{string.Join(", ", routingKeys)}'");
        }
    }
}
