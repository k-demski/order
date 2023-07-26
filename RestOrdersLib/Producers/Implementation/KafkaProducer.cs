using Confluent.Kafka;
using RestOrdersLib.Models;
using System.Text.Json;

namespace RestOrdersLib.Producers
{
    public class KafkaProducer : IKafkaProducer
    {
        public async Task Send(OrderRequest order)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_SERVER"),
            };

            string message = JsonSerializer.Serialize(order);

            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                var result = producer.ProduceAsync(Environment.GetEnvironmentVariable("KAFKA_TOPIC"), new Message<Null, string>() { Value = message })
                    .GetAwaiter()
                    .GetResult();
            }
        }
    }
}
