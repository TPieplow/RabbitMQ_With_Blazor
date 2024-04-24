using Receiver.Broker;

var broker = new RabbitMqBroker("Receiver", "amqp://guest:guest@localhost:5672", "subscribe", "subscriber", "newsletter");

broker.Subscribe();
Console.ReadKey();

broker.Close();