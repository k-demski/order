using RestOrdersLib.Models;

namespace RestOrdersLib.Repositories
{
    public interface IOrderAggregateRepository
    {
        OrderAggregate GetOrderAggregate(int orderId);
    }
}
