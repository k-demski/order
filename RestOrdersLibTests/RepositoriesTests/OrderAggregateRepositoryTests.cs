using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using RestOrdersLib.Models.DatabaseModels;
using RestOrdersLib.Repositories;

namespace RestOrdersLibTests.RepositoriesTests
{
    internal class OrderAggregateRepositoryTests
    {
        private Mock<MongoClient> mongoClientMock;
        private Mock<IMongoDatabase> mongoDatabaseMock;
        private Mock<IMongoCollection<OrderDetailDb>> ordersCollectionMock;
        private Mock<OrderDetailDb> orderDetailDbMock;
        private OrderRepository ordersRepository;
        private OrderDetailDb orderDetailDb;

        [SetUp]
        public void Initialize()
        {
            mongoClientMock = new Mock<MongoClient>();
            mongoDatabaseMock = new Mock<IMongoDatabase>();
            ordersCollectionMock = new Mock<IMongoCollection<OrderDetailDb>>();
            orderDetailDbMock = new Mock<OrderDetailDb> { };
            ordersCollectionMock.Setup(o => o.InsertOne(orderDetailDbMock.Object, It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()));
            mongoClientMock.Setup(o => o.GetDatabase(It.IsAny<string>(), default)).Returns(mongoDatabaseMock.Object);
            mongoDatabaseMock.Setup(o => o.GetCollection<OrderDetailDb>(It.IsAny<string>(), default)).Returns(ordersCollectionMock.Object);

            Mock<IOptions<OrderDatabaseSettings>> mock = new Mock<IOptions<OrderDatabaseSettings>>();

            OrderDatabaseSettings settings = new OrderDatabaseSettings()
            {
                ConnectionString = "mongodb://mongodborderdetail:27017",
                DatabaseName = "mongodborderdetail",
                OrderDetailCollectionName = "orders"
            };

            Mock<IOptions<OrderDatabaseSettings>> orderDatabaseSettings = new Mock<IOptions<OrderDatabaseSettings>>();
            orderDatabaseSettings.Setup(o => o.Value).Returns(settings);

            ordersRepository = new OrderRepository(orderDatabaseSettings.Object);
            orderDetailDb = new OrderDetailDb();
        }

        [Test]
        [Ignore("Timeout on InsertOne.")]
        public void AddMethodShouldSaveToDb()
        {
            ordersRepository.Add(orderDetailDb);
            ordersCollectionMock.Verify(m => m.InsertOne(orderDetailDbMock.Object, It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
