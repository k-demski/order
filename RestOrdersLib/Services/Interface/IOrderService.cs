using RestOrdersLib.Models;

namespace RestOrdersLib.Services
{
    public interface IOrderService
    {
        void AddEvent(OrderRequest order);
    }
}
