using RestOrdersLib.Models;

namespace RestOrdersLib.Producers
{
    public interface IRabbitMQProducer
    {
        public void Send(OrderRequest order);
    }
}
