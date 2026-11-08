namespace VynilServer.Domain.Entities;

public class Order : Entity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public DateTime Date { get; set; } = DateTime.UtcNow;
}

public class OrderItem
{
    
}