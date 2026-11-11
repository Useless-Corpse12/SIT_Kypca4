namespace VynilServer.Domain.Entities;

public class Order : Entity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public DateTime Date { get; set; }
    public decimal TotalPrice { get; set; }
    
    public ICollection<OrderItem> OrderItems { get; set; } = [];
    
    private Order() {}

    public static Order Create(Guid userId)
    {
        return new Order { 
            Id = Guid.NewGuid(),
            UserId = userId,
            Date = DateTime.UtcNow,
            TotalPrice = 0
        };

    }
}

public class OrderItem : Entity
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    
    private OrderItem() {}

    public static OrderItem Create(Guid orderId, Guid productId, decimal price, int quantity)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity must be > 0");
        if (price < 0) throw new ArgumentException("Price must be >= 0");
        
        return new OrderItem
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            ProductId = productId,
            Price = price,
            Quantity = quantity
        };
    }
}