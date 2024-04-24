using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Diagnostics;
using System.Text;

namespace Blazor_Client.Receiver.Brokers
{
    public class RabbitMqBroker
    {
        private readonly string _queueName;
        private readonly string _exchangeName;
        private readonly string _routingKey;

        private readonly ConnectionFactory _factory;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private EventingBasicConsumer _consumer;

        public RabbitMqBroker(string clientProvidedName, string connectionString, string queueName, string exchangeName, string routingKey)
        {
            _queueName = queueName;
            _exchangeName = exchangeName;
            _routingKey = routingKey;

            _factory = new ConnectionFactory()
            {
                Uri = new Uri(connectionString),
                ClientProvidedName = clientProvidedName,
            };

            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(_exchangeName, ExchangeType.Direct);
            _channel.QueueDeclare(_queueName, false, false, false, null);
            _channel.QueueBind(queueName, _exchangeName, _routingKey);
            _channel.BasicQos(0, 1, false);

            _consumer = new EventingBasicConsumer(_channel);
        }

        public void Close()
        {
            _channel.BasicCancel(_channel.BasicConsume(_queueName, false, _consumer));
            _channel.Close();
            _connection.Close();
        }

        // kan lägga till string queueName om man vill ändra kön
        public void Subscribe()
        {
            try
            {
                _consumer.Received += (sender, args) =>
                {
                    string message = Encoding.UTF8.GetString(args.Body.ToArray());
                    Console.WriteLine($" Received: {message}");

                    _channel.BasicAck(args.DeliveryTag, false);
                };
                _channel.BasicConsume(_queueName, false, _consumer);
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); }
        }
       
        public bool IsSubscribed { get; set; } = false;
        public void Subscribe(Action<string> messageReceivedCallback)
        {
            try
            {
                _consumer.Received += (sender, args) =>
                {
                    string message = Encoding.UTF8.GetString(args.Body.ToArray());
                    messageReceivedCallback(message);
                    _channel.BasicAck(args.DeliveryTag, false);
                };
                _channel.BasicConsume(_queueName, false, _consumer);
                IsSubscribed = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
