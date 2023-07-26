using OrdersEventHandler.Models;
using OrdersEventHandler.Models.DatabaseModels;
using OrdersEventHandler.Repositories;
using System.ComponentModel.DataAnnotations;

namespace OrdersEventHandler.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            this.orderRepository = orderRepository;
        }

        public void ProceedOrderNotification(OrderNotification order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order), "Order should not be null.");
            }

            if (order.OrderId <= 0)
            {
                throw new ValidationException("OrderId should be greater than zero.");
            }

            if (string.IsNullOrEmpty(order.ProductName))
            {
                throw new ValidationException("Product should not be null.");
            }

            if (order.Price <= 0)
            {
                throw new ValidationException("Price should be greater than zero.");
            }

            if (order.Quantity <= 0)
            {
                throw new ValidationException("Quantity should be greater than zero.");
            }

            // try find order by orderId
            OrderDb orderDb = orderRepository.FindOrderByOrderId(order.OrderId);

            // order not found then creating new order with product
            if (orderDb == null)
            {
                orderDb = CreateOrder(order);
                orderRepository.Add(orderDb);
                return;
            }

            // try find product
            ProductDb productDb = orderDb.Products.Where(p => p.Name == order.ProductName).FirstOrDefault();

            // product not found then creating new product
            if (productDb == null)
            {
                AddProduct(orderDb, order);
                orderRepository.Update(orderDb);
                return;
            }

            RecalculateProduct(productDb, order);
            orderRepository.Update(orderDb);
        }

        private OrderDb CreateOrder(OrderNotification order)
        {
            OrderDb orderDb = new OrderDb
            {
                OrderId = order.OrderId,
                Products = new List<ProductDb>(),
            };

            AddProduct(orderDb, order);

            return orderDb;
        }

        private void AddProduct(OrderDb orderDb, OrderNotification order)
        {
            orderDb.Products.Add(new ProductDb()
            {
                Name = order.ProductName,
                Price = order.Price,
                Quantity = order.Quantity
            });
        }

        private void RecalculateProduct(ProductDb product, OrderNotification order)
        {
            if (order.EventType == OrderEventType.Add.ToString())
            {
                product.Quantity += order.Quantity;
            }

            if (order.EventType == OrderEventType.Remove.ToString())
            {
                product.Quantity -= order.Quantity;
            }
        }
    }
}