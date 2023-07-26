using Microsoft.Extensions.Caching.Distributed;
using RestOrdersLib.Models;
using RestOrdersLib.Models.DatabaseModels;
using RestOrdersLib.Repositories;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace RestOrdersLib.Services
{
    public class OrderService : IOrderService
    {
        private readonly IDistributedCache redisCache;
        private readonly IOrderAggregateRepository orderAggregateRepository;
        private readonly IOrderRepository orderRepository;

        public OrderService(IDistributedCache redisCache,
            IOrderAggregateRepository orderAggregateRepository,
            IOrderRepository orderRepository)
        {
            this.redisCache = redisCache;
            this.orderAggregateRepository = orderAggregateRepository;
            this.orderRepository = orderRepository;
        }

        public void AddEvent(OrderRequest orderRequest)
        {
            if (orderRequest == null)
            {
                throw new ArgumentNullException(nameof(orderRequest), "OrderRequest should not be null.");
            }

            if (orderRequest.OrderId <= 0)
            {
                throw new ValidationException("OrderId should be greater than zero.");
            }

            if (orderRequest.EventType != OrderEventType.Add.ToString() && orderRequest.EventType != OrderEventType.Remove.ToString())
            {
                throw new ValidationException("EventType should be Add or Remove.");
            }

            if (string.IsNullOrEmpty(orderRequest.ProductName))
            {
                throw new ValidationException("ProductName should not be null.");
            }

            if (orderRequest.Price <= 0)
            {
                throw new ValidationException("Price should be greater than zero.");
            }

            if (orderRequest.Quantity <= 0)
            {
                throw new ValidationException("Quantity should be greater than zero.");
            }

            // Get actual OrderAggregate
            OrderAggregate orderAggregate = GetOrderAggregate(orderRequest.OrderId);

            // Validate that save to MongoDB and apply event on OrderAggregate is possible
            // TODO:

            // Save to MongoDB
            OrderDetailDb orderDetailDb = ConvertToDbModel(orderRequest);
            orderRepository.Add(orderDetailDb);

            // Apply new event on OrderAggregate and save to Redis cache
            ApplyEventOnOrderAggregate(orderAggregate, orderRequest);
            string orderAggregateStringToSave = JsonSerializer.Serialize(orderAggregate);
            redisCache.SetString(orderRequest.OrderId.ToString(), orderAggregateStringToSave);
        }

        private OrderAggregate GetOrderAggregate(int orderId)
        {
            OrderAggregate orderAggregate;

            // try to get OrderAggregate from Redis cache
            string orderAggregateString = redisCache.GetString(orderId.ToString());

            if (string.IsNullOrEmpty(orderAggregateString))
            {
                // if OrderAggregate not exists in cache try to Get OrderAggregate from MongoDB
                orderAggregate = orderAggregateRepository.GetOrderAggregate(orderId);
            }
            else
            {
                orderAggregate = JsonSerializer.Deserialize<OrderAggregate>(orderAggregateString);
            }

            return orderAggregate;
        }

        private void ApplyEventOnOrderAggregate(OrderAggregate orderAggregate, OrderRequest order)
        {
            if (orderAggregate.OrderId == 0)
            {
                orderAggregate.OrderId = order.OrderId;
                orderAggregate.Products = new List<Product>();
            }
            
            Product product = orderAggregate.Products.Where(o => o.Name == order.ProductName).FirstOrDefault();

            if (product == null)
            {
                orderAggregate.Products.Add(product = new Product() { Name = order.ProductName, Price = order.Price, Quantity = order.Quantity });
            }
            else
            {
                RecalculateProduct(product, order);
            }
        }

        private void RecalculateProduct(Product product, OrderRequest order)
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

        private OrderDetailDb ConvertToDbModel(OrderRequest order)
        {
            OrderDetail orderEvent = new OrderDetail()
            {
                ProductName = order.ProductName,
                Price = order.Price,
                Quantity = order.Quantity
            };

            return new OrderDetailDb
            {
                OrderId = order.OrderId,
                CreatedTime = DateTime.Now,
                EventType = order.EventType,
                OrderDetails = JsonSerializer.Serialize(orderEvent)
            };
        }
    }
}