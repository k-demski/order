using OrdersEventHandler.Models.DatabaseModels;

namespace OrdersEventHandler.Repositories
{
    public interface IProductRepository
    {
        ProductDb FindByNameAndPrice(string name, decimal price);
    }
}
