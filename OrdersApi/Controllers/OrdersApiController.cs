using Microsoft.AspNetCore.Mvc;
using OrdersApi.Data;
using OrdersApi.Models;
using OrdersApi.Utils;

namespace OrdersApi.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersApiController : ControllerBase
    {
        private readonly ILogger<OrdersApiController> _logger;
        private readonly OrdersDbContext _dbContext;
        private readonly IRequestValidator<Order> _orderValidator;


        public OrdersApiController(ILogger<OrdersApiController> logger, OrdersDbContext dbContext, IRequestValidator<Order> orderValidator)
        {
            _logger = logger;
            _dbContext = dbContext;
            _orderValidator = orderValidator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrderAsync([FromBody] Order order)
        {
            if (!_orderValidator.Validate(order, out IActionResult? errorResponse) && errorResponse != null)
            {
                _logger.LogError("Received invalid order: {@Order}. Error: {errorResponse}", order, errorResponse);
                return errorResponse;
            }

            _logger.LogInformation("Received valid order: {@Order}", order);

            try
            {
                await _dbContext.Orders.AddAsync(order);
                var result = await _dbContext.SaveChangesAsync();

                if (result > 0)
                {
                    _logger.LogInformation("Order created successfully: {@Order}", order);
                    return StatusCode(201, new { order.OrderId });
                }
                else
                {
                    _logger.LogError($"Failed to save order to db for OrderId: {order.OrderId}");
                    return StatusCode(500, "Database error. Failed to save order to db.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error saving order: {ex.Message} for OrderId: {order.OrderId}");
                return StatusCode(500, new
                {
                    error = "Exception when saving order to db.",
                    message = ex.Message
                });
            }
        }
    }
}
