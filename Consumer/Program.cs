using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

//Consumer

var factory = new ConnectionFactory { HostName= "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare("headersexchange", type: ExchangeType.Headers);

channel.QueueDeclare(queue: "letterbox2");
var bindingArgument = new Dictionary<string, object>
{
    {"x-match", "all" },
    {"name", "brian" },
    {"age", "31" },
};
channel.QueueBind("letterbox2", "headersexchange", "", bindingArgument);

var consumer = new EventingBasicConsumer(channel);

consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Received new message: {message}");
};

channel.BasicConsume(queue: "letterbox2", autoAck: true, consumer: consumer);

Console.WriteLine("Consuming");
Console.ReadKey();