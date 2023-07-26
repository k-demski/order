using FluentAssertions;
using Moq;
using OrdersEventHandler.Models;
using OrdersEventHandler.Repositories;
using OrdersEventHandler.Services;
using System.ComponentModel.DataAnnotations;

namespace OrdersEventHandlerTests.ServicesTests
{
    internal class OrderServiceTests
    {
        private Mock<IOrderRepository> orderRepositoryMock;
        private OrderService orderService;
        private OrderNotification order;

        [SetUp]
        public void Initialize()
        {
            orderRepositoryMock = new Mock<IOrderRepository>();
            order = new OrderNotification();
        }

        [Test]
        public void OrderShouldNotBeNull()
        {
            orderService = new OrderService(orderRepositoryMock.Object);

            Action act = () => orderService.ProceedOrderNotification(null);
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName(nameof(order))
                .WithMessage("Order should not be null. (Parameter 'order')");
        }

        [Test]
        public void OrderIdShouldBeGraeterThanZero()
        {
            orderService = new OrderService(orderRepositoryMock.Object);

            Action act = () => orderService.ProceedOrderNotification(order);
            act.Should().Throw<ValidationException>()
                .WithMessage("OrderId should be greater than zero.");
        }

        [Test]
        public void OrderShouldHaveProduct()
        {
            orderService = new OrderService(orderRepositoryMock.Object);

            order.OrderId = 1;

            Action act = () => orderService.ProceedOrderNotification(order);
            act.Should().Throw<ValidationException>()
                .WithMessage("Product should not be null.");
        }

        [Test]
        public void PriceShouldBeGraeterThanZero()
        {
            orderService = new OrderService(orderRepositoryMock.Object);

            order.OrderId = 1;
            order.ProductName = "Tea";

            Action act = () => orderService.ProceedOrderNotification(order);
            act.Should().Throw<ValidationException>()
                .WithMessage("Price should be greater than zero.");
        }

        [Test]
        public void QuantityShouldBeGraeterThanZero()
        {
            orderService = new OrderService(orderRepositoryMock.Object);

            order.OrderId = 1;
            order.ProductName = "Tea";
            order.Price = 10;

            Action act = () => orderService.ProceedOrderNotification(order);
            act.Should().Throw<ValidationException>()
                .WithMessage("Quantity should be greater than zero.");
        }

        [Test]
        public void ProceedOrderNotificationMethodShouldSaveToDb()
        {
            orderService = new OrderService(orderRepositoryMock.Object);

            order.OrderId = 1;
            order.ProductName = "Tea";
            order.Price = 10;
            order.Quantity = 1;

            orderService.ProceedOrderNotification(order);

            // TODO: dokończyć
        }
    }
}
