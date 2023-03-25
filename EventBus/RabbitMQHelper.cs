using System.Diagnostics;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RegistrationApi.EventBus
{
    public class RabbitMQHelper<T>
    {
        public event EventHandler<T>? OnReceive;
        private readonly IConnection connection;
        private readonly string queue;
        private readonly string exchange;

        public RabbitMQHelper(string host, string queue, string exchange)
        {
            var factory = new ConnectionFactory
            {
                HostName = host
            };
            this.connection = factory.CreateConnection();
            this.queue = queue;
            this.exchange = exchange;
        }

        public void Publish(T message, params string[] routingKeys)
        {
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange, ExchangeType.Topic, durable: true);

            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            foreach (var key in routingKeys)
            {
                channel.BasicPublish(exchange, key, properties, body);
            }

            Debug.WriteLine($" [x] Sent '{message}' to '{routingKeys}'");
        }

        public void Subscribe(params string[] routingKeys)
        {
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.ExchangeDeclare(exchange, ExchangeType.Topic, durable: true);

            foreach (var key in routingKeys)
            {
                channel.QueueBind(queue, exchange, key);
            }

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += HandleMessage;

            channel.BasicConsume(queue, true, consumer: consumer);

            Debug.WriteLine($" [*] Listening on '{routingKeys}'...");
        }

        private void HandleMessage(object model, BasicDeliverEventArgs eventArgs)
        {
            var json = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
            var message = JsonConvert.DeserializeObject<T>(json);

            Debug.WriteLine($" [x] Received '{message}' from '{eventArgs.RoutingKey}'");

            OnReceive?.Invoke(this, message);
        }
    }
}
