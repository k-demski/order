using Confluent.Kafka;
using OrdersEventHandler.Models;
using OrdersEventHandler.Services;
using System.Text.Json;

namespace OrdersEventHandler.Consumers
{
    public class KafkaConsumer : IHostedService
    {
        protected readonly IOrderService orderService;

        public KafkaConsumer(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            ConsumerConfig conf = new ConsumerConfig
            {
                GroupId = Environment.GetEnvironmentVariable("KAFKA_GROUPID"),
                BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_SERVER"),
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            
            using (IConsumer<Ignore, string> builder = new ConsumerBuilder<Ignore, string>(conf).Build())
            {
                builder.Subscribe(Environment.GetEnvironmentVariable("KAFKA_TOPIC"));
                CancellationTokenSource cancelToken = new CancellationTokenSource();
                try
                {
                    while (true)
                    {
                        var consumer = builder.Consume(cancelToken.Token);

                        Console.WriteLine($"Message: {consumer.Message.Value} received from {consumer.TopicPartitionOffset}");

                        OrderNotification order = JsonSerializer.Deserialize<OrderNotification>(consumer.Message.Value);

                        orderService.ProceedOrderNotification(order);
                    }
                }
                catch (Exception ex)
                {
                    builder.Close();
                }
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
