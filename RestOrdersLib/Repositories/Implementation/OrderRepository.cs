using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RestOrdersLib.Models.DatabaseModels;

namespace RestOrdersLib.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IMongoCollection<OrderDetailDb> ordersCollection;

        public OrderRepository(IOptions<OrderDatabaseSettings> orderDatabaseSettings)
        {
            MongoClient mongoClient = new MongoClient(orderDatabaseSettings.Value.ConnectionString);

            IMongoDatabase mongoDatabase = mongoClient.GetDatabase(orderDatabaseSettings.Value.DatabaseName);

            ordersCollection = mongoDatabase.GetCollection<OrderDetailDb>(orderDatabaseSettings.Value.OrderDetailCollectionName);
        }

        public void Add(OrderDetailDb orderDetailDb)
        {
            ordersCollection.InsertOne(orderDetailDb);
        }
    }
}
