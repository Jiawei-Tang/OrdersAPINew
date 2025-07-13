using Microsoft.EntityFrameworkCore;
using OrdersApi.Data;

public class FailingOrdersDbContext : OrdersDbContext
{
    public FailingOrdersDbContext(DbContextOptions<OrdersDbContext> options)
    : base(options)
    {
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        throw new Exception("Simulated DB failure");
    }
}
