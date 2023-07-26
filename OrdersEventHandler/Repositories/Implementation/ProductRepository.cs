using OrdersEventHandler.Models.DatabaseModels;

namespace OrdersEventHandler.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly OrderDbContext _context;

        public ProductRepository(OrderDbContext context)
        {
            _context = context;
        }

        public ProductDb FindByNameAndPrice(string name, decimal price)
        {
            return _context.Products.Where(p => p.Name == name && p.Price == price).FirstOrDefault();
        }
    }
}