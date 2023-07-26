using RabbitMQ.Client;
using RestOrdersLib.Models;
using System.Text.Json;

namespace RestOrdersLib.Producers
{
    public class RabbitMQProducer : IRabbitMQProducer
    {
        public void Send(OrderRequest order)
        {
            string message = JsonSerializer.Serialize(order);

            int port;
            int.TryParse(Environment.GetEnvironmentVariable("RABBITMQ_PORT"), out port);

            ConnectionFactory factory = new ConnectionFactory()
            {
                HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST"),
                Port = port,
                UserName = Environment.GetEnvironmentVariable("RABBITMQ_USERNAME"),
                Password = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD"),
            };

            using (IConnection connection = factory.CreateConnection())
            using (IModel channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "order", type: "fanout");

                byte[] body = System.Text.Encoding.UTF8.GetBytes(message);

                Console.WriteLine("Send message: {0}", message);

                channel.BasicPublish(exchange: "order", routingKey: "", basicProperties: null, body: body);
            }
        }
    }
}
