using Microsoft.AspNetCore.Mvc;

namespace OrdersApi.Utils
{
    public interface IRequestValidator<T>
    {
        bool Validate(T request, out IActionResult? errorResponse);
    }
}
