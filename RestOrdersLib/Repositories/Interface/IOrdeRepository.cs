using RestOrdersLib.Models.DatabaseModels;

namespace RestOrdersLib.Repositories
{
    public interface IOrderRepository
    {
        void Add(OrderDetailDb orderDetailDb);
    }
}
