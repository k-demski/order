using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using OrdersEventHandler.Models;
using System.Text.Json;
using OrdersEventHandler.Services;

namespace OrdersEventHandler.Consumers
{
    public class RabbitMQConsumer : BackgroundService
    {
        private string queueName;
        private ConnectionFactory factory;
        private IConnection connection;
        private IModel channel;
        protected readonly IOrderService orderService;
        public RabbitMQConsumer(IOrderService orderService)
        {
            int port;
            int.TryParse(Environment.GetEnvironmentVariable("RABBITMQ_PORT"), out port);

            factory = new ConnectionFactory()
            {
                HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST"),
                Port = port,
                UserName = Environment.GetEnvironmentVariable("RABBITMQ_USERNAME"),
                Password = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD"),
            };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: queueName, exchange: "order", routingKey: "");

            this.orderService = orderService;
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                channel.Dispose();
                connection.Dispose();
                return Task.CompletedTask;
            }

            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                byte[] body = ea.Body.ToArray();
                string message = System.Text.Encoding.UTF8.GetString(body);

                Console.WriteLine("Receive message: {0}", message);

                OrderNotification order = JsonSerializer.Deserialize<OrderNotification>(message);

                orderService.ProceedOrderNotification(order);
            };

            channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }
    }
}
