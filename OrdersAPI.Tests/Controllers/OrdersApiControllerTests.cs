using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using OrdersApi.Controllers;
using OrdersApi.Data;
using OrdersApi.Models;
using OrdersApi.Utils;

namespace OrdersAPI.Tests.Controllers
{
    [TestClass]
    public sealed class OrdersApiControllerTests
    {
        private readonly OrderRequestValidator _validator = new();
        private OrdersApiController _controller;
        private OrdersDbContext _dbContext;
        private Mock<ILogger<OrdersApiController>> _mockLogger;

        [TestInitialize]
        public void SetUp()
        {

            var options = new DbContextOptionsBuilder<OrdersDbContext>()
             .UseInMemoryDatabase(databaseName: "OrdersTestDb")
             .Options;

            _dbContext = new OrdersDbContext(options);

            _mockLogger = new Mock<ILogger<OrdersApiController>>();

            _controller = new OrdersApiController(_mockLogger.Object, _dbContext, _validator);
        }

        [TestMethod]
        public async Task CreateOrderAsync_ValidOrder_ReturnsCreated()
        {
            var order = new Order
            {
                OrderId = Guid.NewGuid(),
                CustomerName = "TestUser",
                CreatedAt = DateTime.UtcNow,
                Items = new List<OrderItem> { new() { ProductId = Guid.NewGuid(), Quantity = 1 } }
            };

            var result = await _controller.CreateOrderAsync(order);

            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var objectResult = result as ObjectResult;
            Assert.AreEqual(201, objectResult?.StatusCode);

            _mockLogger.Verify(
             x => x.Log(
             LogLevel.Information,
             It.IsAny<EventId>(),
             It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Order created successfully")),
             It.IsAny<Exception>(),
             It.IsAny<Func<It.IsAnyType, Exception, string>>()),
             Times.Once);
        }

        [TestMethod]
        public async Task CreateOrderAsync_InvalidOrder_ReturnsBadRequest()
        {
            // Arrange
            var order = new Order
            {
                OrderId = Guid.NewGuid(),
                CustomerName = String.Empty,
                CreatedAt = DateTime.UtcNow,
                Items = new List<OrderItem> { new() { ProductId = Guid.NewGuid(), Quantity = 1 } }
            };

            // Act
            var result = await _controller.CreateOrderAsync(order);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequest = result as BadRequestObjectResult;
            Assert.AreEqual("Customer name must not be empty.", badRequest?.Value);

            _mockLogger.Verify(
             x => x.Log(
             LogLevel.Error,
             It.IsAny<EventId>(),
             It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Received invalid order")),
             It.IsAny<Exception>(),
             It.IsAny<Func<It.IsAnyType, Exception, string>>()),
             Times.Once);
        }

        [TestMethod]
        public async Task CreateOrderAsync_DbThrowsException_Returns500()
        {
            // Arrange
            var order = new Order
            {
                OrderId = Guid.NewGuid(),
                CustomerName = "TestUser",
                CreatedAt = DateTime.UtcNow,
                Items = new List<OrderItem> { new() { ProductId = Guid.NewGuid(), Quantity = 1 } }
            };

            var options = new DbContextOptionsBuilder<OrdersDbContext>()
             .UseInMemoryDatabase("FailingDb")
             .Options;

            var failingDbContext = new FailingOrdersDbContext(options);
            var mockLogger = new Mock<ILogger<OrdersApiController>>();
            var controller = new OrdersApiController(mockLogger.Object, failingDbContext, _validator);

            // Act
            var result = await controller.CreateOrderAsync(order);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult?.StatusCode);
            string errorMessage = objectResult?.Value?.ToString() ?? string.Empty;
            Assert.IsTrue(errorMessage.Contains("Simulated DB failure"));
            Assert.IsTrue(errorMessage.Contains("Exception when saving order to db."));

            mockLogger.Verify(
             x => x.Log(
             LogLevel.Error,
             It.IsAny<EventId>(),
             It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error saving order")),
             It.IsAny<Exception>(),
             It.IsAny<Func<It.IsAnyType, Exception, string>>()),
             Times.Once);
        }
    }
}
