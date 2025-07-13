using Microsoft.AspNetCore.Mvc;
using OrdersApi.Models;

namespace OrdersApi.Utils
{
    public class OrderRequestValidator : IRequestValidator<Order>
    {
        // VerifyRequestBody. Return false if body is null, or if any parameter is null, or CreatedAt is over 15 minutes ago. Also output a response when failed.
        public bool Validate(Order order, out IActionResult? errorResponse)
        {
            errorResponse = null;

            if (order == null)
            {
                errorResponse = new BadRequestObjectResult("Order must not be null.");
                return false;
            }

            if (order.OrderId == Guid.Empty)
            {
                errorResponse = new BadRequestObjectResult("Order ID must not be null.");
                return false;
            }

            if (string.IsNullOrEmpty(order.CustomerName))
            {
                errorResponse = new BadRequestObjectResult("Customer name must not be empty.");
                return false;
            }

            if (order.CreatedAt < DateTime.UtcNow.AddMinutes(-15) || order.CreatedAt > DateTime.UtcNow)
            {
                errorResponse = new BadRequestObjectResult("Order created time must be within past 15 minutes.");
                return false;
            }

            if (order.Items == null || !order.Items.Any())
            {
                errorResponse = new BadRequestObjectResult("Items must not be empty.");
                return false;
            }

            return true;
        }
    }
}
