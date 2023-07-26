using RestOrdersLib.Models;

namespace RestOrdersLib.Producers
{
    public interface IKafkaProducer
    {
        public Task Send(OrderRequest order);
    }
}
