using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RegistrationService.RabbitMQ.Connection;
using System.Diagnostics;
using System.Text;

namespace RegistrationService.RabbitMQ
{
    public class RabbitMQListener<T> : IRabbitMQListener<T>
    {
        public event EventHandler<T> OnReceive;

        public RabbitMQListener(IRabbitMQConnection connection, string queue, string exchange, params string[] routingKeys)
        {
            try
            {
                var client = connection.TryConnect();

                var channel = client.CreateModel();

                channel.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false, arguments: null);
                channel.ExchangeDeclare(exchange, ExchangeType.Topic, durable: true);

                foreach (var key in routingKeys)
                {
                    channel.QueueBind(queue, exchange, key);
                }

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += HandleMessage;

                channel.BasicConsume(queue, true, consumer: consumer);

                Debug.WriteLine($" [x] Listening on '{string.Join(", ", routingKeys)}'...");
            } catch { }
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
