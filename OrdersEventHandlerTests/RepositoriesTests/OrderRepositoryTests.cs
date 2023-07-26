using Microsoft.EntityFrameworkCore;
using Moq;
using OrdersEventHandler.Models.DatabaseModels;
using OrdersEventHandler.Repositories;

namespace OrdersEventHandlerTests.RepositoriesTests
{
    internal class OrderRepositoryTests
    {
        private Mock<DbSet<OrderDb>> dbSetOrdersDbMock;
        private Mock<OrderDbContext> orderDbContextMock;
        private OrderRepository orderRepository;
        private OrderDb orderDb;

        [SetUp]
        public void Initialize()
        {
            dbSetOrdersDbMock = new Mock<DbSet<OrderDb>>();
            var options = new DbContextOptionsBuilder<OrderDbContext>().Options;
            orderDbContextMock = new Mock<OrderDbContext>(options);
            orderDbContextMock.Setup(m => m.Orders).Returns(dbSetOrdersDbMock.Object);

            orderRepository = new OrderRepository(orderDbContextMock.Object);
            orderDb = new OrderDb();
        }

        [Test]
        public void AddMethodShouldSaveToDb()
        {
            orderRepository.Add(orderDb);

            dbSetOrdersDbMock.Verify(m => m.Add(It.IsAny<OrderDb>()), Times.Once());
            orderDbContextMock.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Test]
        public void UpdateMethodShouldSaveToDb()
        {
            orderRepository.Update(orderDb);

            dbSetOrdersDbMock.Verify(m => m.Update(It.IsAny<OrderDb>()), Times.Once());
            orderDbContextMock.Verify(m => m.SaveChanges(), Times.Once());
        }
    }
}
