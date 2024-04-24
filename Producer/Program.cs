using Producer.Brokers;

var broker = new RabbitMqBroker("Producer App", "amqp://guest:guest@localhost:5672", "subscribe", "subscriber", "newsletter");

bool repeat = true;
do
{
    Console.Clear();
    Console.Write("Do you want to subscribe? (y/n): ");
    var answer = Console.ReadLine();
    
    if (answer!.Equals("y", StringComparison.CurrentCultureIgnoreCase))
    {
        Console.Clear();
        Console.Write("Enter email to subscribe: ");
        var email = Console.ReadLine();

        if (!string.IsNullOrEmpty(email))
        {
            var result = broker.Publish(email);
            var message = result ? "Message was sent" : "Something went wrong";

            Console.WriteLine(message);
            Console.ReadKey();
        }
    }
    else
    {
        repeat = false;
    }
} while (repeat);

broker.Close();