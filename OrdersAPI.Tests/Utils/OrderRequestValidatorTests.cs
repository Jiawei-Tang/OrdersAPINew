using Microsoft.AspNetCore.Mvc;
using OrdersApi.Models;
using OrdersApi.Utils;

namespace OrdersAPI.Tests.Utils
{
    [TestClass]
    public sealed class OrderRequestValidatorTests
    {
        private readonly OrderRequestValidator _validator = new();

        [TestMethod]
        public void Validate_ReturnsFalse_WhenOrderIsNull()
        {
            var result = _validator.Validate(null, out var response);

            Assert.IsFalse(result);
            Assert.IsInstanceOfType(response, typeof(BadRequestObjectResult));
            Assert.AreEqual("Order must not be null.", ((BadRequestObjectResult)response).Value);
        }

        [TestMethod]
        public void Validate_ReturnsTrue_WhenOrderIsValid()
        {
            var order = new Order
            {
                OrderId = Guid.NewGuid(),
                CustomerName = "TestUser",
                CreatedAt = DateTime.UtcNow,
                Items = new List<OrderItem> { new() { ProductId = Guid.NewGuid(), Quantity = 1 } }
            };

            var result = _validator.Validate(order, out var response);

            Assert.IsTrue(result);
            Assert.IsNull(response);
        }

        [TestMethod]
        public void Validate_ReturnsFalse_WhenOrderParameterIsNotLegal()
        {
            var order = new Order
            {
                OrderId = Guid.Empty,
                CustomerName = "TestUser",
                CreatedAt = DateTime.UtcNow.AddMinutes(-1),
                Items = new List<OrderItem> { new() { ProductId = Guid.NewGuid(), Quantity = 1 } }
            };

            var result = _validator.Validate(order, out var response);
            Assert.IsFalse(result);
            Assert.IsInstanceOfType(response, typeof(BadRequestObjectResult));
            Assert.AreEqual("Order ID must not be null.", ((BadRequestObjectResult)response).Value);

            order.OrderId = Guid.NewGuid();
            order.CustomerName = string.Empty;
            result = _validator.Validate(order, out response);
            Assert.IsFalse(result);
            Assert.IsInstanceOfType(response, typeof(BadRequestObjectResult));
            var badRequest = (BadRequestObjectResult)response;
            Assert.AreEqual("Customer name must not be empty.", badRequest.Value);

            order.CustomerName = "TestUser";
            order.CreatedAt = DateTime.UtcNow.AddMinutes(-20);
            result = _validator.Validate(order, out response);
            Assert.IsFalse(result);
            Assert.IsInstanceOfType(response, typeof(BadRequestObjectResult));
            Assert.AreEqual("Order created time must be within past 15 minutes.", ((BadRequestObjectResult)response).Value);

            order.CreatedAt = DateTime.UtcNow.AddMinutes(-1);
            order.Items = new List<OrderItem> { };
            result = _validator.Validate(order, out response);
            Assert.IsFalse(result);
            Assert.IsInstanceOfType(response, typeof(BadRequestObjectResult));
            Assert.AreEqual("Items must not be empty.", ((BadRequestObjectResult)response).Value);
        }
    }
}
