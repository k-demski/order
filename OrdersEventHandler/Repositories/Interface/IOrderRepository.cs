using OrdersEventHandler.Models.DatabaseModels;

namespace OrdersEventHandler.Repositories
{
    public interface IOrderRepository
    {
        void Add(OrderDb orderDb);

        OrderDb FindOrderByOrderId(int orderId);

        void Update(OrderDb orderDb);
    }
}
