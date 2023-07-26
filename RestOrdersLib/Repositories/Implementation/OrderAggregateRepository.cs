using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RestOrdersLib.Models;
using RestOrdersLib.Models.DatabaseModels;
using System.Text.Json;

namespace RestOrdersLib.Repositories
{
    public class OrderAggregateRepository : IOrderAggregateRepository
    {
        private readonly IMongoCollection<OrderDetailDb> ordersCollection;

        public OrderAggregateRepository(IOptions<OrderDatabaseSettings> orderDatabaseSettings)
        {
            MongoClient mongoClient = new MongoClient(
                orderDatabaseSettings.Value.ConnectionString);

            IMongoDatabase mongoDatabase = mongoClient.GetDatabase(
                orderDatabaseSettings.Value.DatabaseName);

            ordersCollection = mongoDatabase.GetCollection<OrderDetailDb>(
                orderDatabaseSettings.Value.OrderDetailCollectionName);
        }

        public OrderAggregate GetOrderAggregate(int orderId)
        {
            var filter = Builders<OrderDetailDb>.Filter.Eq("OrderId", orderId);

            List<OrderDetailDb> orderEvents = ordersCollection.Find(filter).ToList();

            if (!orderEvents.Any())
            {
                return new OrderAggregate();
            }

            OrderAggregate orderAggregate = new OrderAggregate();
            orderAggregate.OrderId = orderId;
            orderAggregate.Products = new List<Product>();

            foreach(OrderDetailDb orderEvent in orderEvents)
            {
                OrderDetail orderDetail = JsonSerializer.Deserialize<OrderDetail>(orderEvent.OrderDetails);
                Product product = new Product
                {
                    Name = orderDetail.ProductName,
                    Price = orderDetail.Price,
                    Quantity = orderDetail.Quantity
                };

                orderAggregate.Products.Add(product);
            }

            return orderAggregate;
        }

        private void Recalculate(OrderAggregate orderAggregate)
        {
            // TODO:
        }
    }
}
