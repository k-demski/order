using Microsoft.EntityFrameworkCore;
using OrdersEventHandler.Models.DatabaseModels;

namespace OrdersEventHandler.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDbContext _context;

        public OrderRepository(OrderDbContext context)
        {
            _context = context;
        }

        public void Add(OrderDb orderDb)
        {
            _context.Orders.Add(orderDb);
            _context.SaveChanges();
        }

        public OrderDb FindOrderByOrderId(int orderId)
        {
            return _context.Orders.Include(p => p.Products).Where(o => o.OrderId == orderId).FirstOrDefault();
        }

        public void Update(OrderDb orderDb)
        {
            _context.Orders.Update(orderDb);
            _context.SaveChanges();
        }
    }
}
