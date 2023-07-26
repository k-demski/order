using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Driver;
using Moq;
using RestOrdersLib.Models;
using RestOrdersLib.Models.DatabaseModels;
using RestOrdersLib.Repositories;
using RestOrdersLib.Services;
using System.ComponentModel.DataAnnotations;

namespace RestOrdersLibTests.ServicesTests
{
    internal class OrderServiceTests
    {
        private Mock<IDistributedCache> redisCacheMock;
        private Mock<IOrderAggregateRepository> orderAggregateRepositoryMock;
        private Mock<IOrderRepository> orderRepositoryMock;
        private Mock<OrderDetailDb> orderDetailDbMock;
        private OrderService orderService;
        private OrderRequest orderRequest;

        [SetUp]
        public void Initialize()
        {
            redisCacheMock = new Mock<IDistributedCache> { };

            orderAggregateRepositoryMock = new Mock<IOrderAggregateRepository> { };
            orderRepositoryMock = new Mock<IOrderRepository> { };
            orderDetailDbMock = new Mock<OrderDetailDb> { };

            orderService = new OrderService(redisCacheMock.Object, orderAggregateRepositoryMock.Object, orderRepositoryMock.Object);
            orderRequest = new OrderRequest();
        }

        [Test]
        public void OrderShouldNotBeNull()
        {
            Action act = () => orderService.AddEvent(null);
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName(nameof(orderRequest))
                .WithMessage("OrderRequest should not be null. (Parameter 'orderRequest')");
        }

        [Test]
        public void OrderIdShouldBeGraeterThanZero()
        {
            Action act = () => orderService.AddEvent(orderRequest);
            act.Should().Throw<ValidationException>()
                .WithMessage("OrderId should be greater than zero.");
        }

        [Test]
        public void OrderShouldHaveCorrectEventType()
        {
            orderRequest.OrderId = 1;

            Action act = () => orderService.AddEvent(orderRequest);
            act.Should().Throw<ValidationException>()
                .WithMessage("EventType should be Add or Remove.");
        }

        [Test]
        public void OrderShouldHaveProductName()
        {
            orderRequest.OrderId = 1;
            orderRequest.EventType = "Add";

            Action act = () => orderService.AddEvent(orderRequest);
            act.Should().Throw<ValidationException>()
                .WithMessage("ProductName should not be null.");
        }

        [Test]
        public void PriceShouldBeGraeterThanZero()
        {
            orderRequest.OrderId = 1;
            orderRequest.EventType = "Add";
            orderRequest.ProductName = "Tea";

            Action act = () => orderService.AddEvent(orderRequest);
            act.Should().Throw<ValidationException>()
                .WithMessage("Price should be greater than zero.");
        }

        [Test]
        public void QuantityShouldBeGraeterThanZero()
        {
            orderRequest.OrderId = 1;
            orderRequest.EventType = "Add";
            orderRequest.ProductName = "Tea";
            orderRequest.Price = 10;

            Action act = () => orderService.AddEvent(orderRequest);
            act.Should().Throw<ValidationException>()
                .WithMessage("Quantity should be greater than zero.");
        }

        [Test]
        [Ignore("TODO:")]
        public void AddMethodShouldSaveToDb()
        {
            orderRequest.OrderId = 1;
            orderRequest.EventType = "Add";
            orderRequest.ProductName = "Tea";
            orderRequest.Price = 10;
            orderRequest.Quantity = 1;

            orderService.AddEvent(orderRequest);

            orderRepositoryMock.Verify(m => m.Add(orderDetailDbMock.Object), Times.Once());
        }
    }
}
