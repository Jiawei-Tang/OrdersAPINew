namespace OrdersApi.Models
{
    public class Order
    {
        public Guid OrderId { get; set; }

        public required string CustomerName { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<OrderItem> Items { get; set; } = new();
    }

    public class OrderItem
    {
        public Guid OrderItemId { get; set; }

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public Guid OrderId { get; set; }
    }
}
