namespace VynilServer.Domain.Entities;

public class Review: Entity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public string Body { get; set; } = string.Empty;
    public byte Rate { get; set; } // 1-10
    
    private Review()
    {
    }

    public Review(Guid productId, Guid userId, string body, int rate)
    {
        UserId = userId;
        ProductId = productId;
        Body = body;
        Rate = (byte)Math.Clamp(rate,1,10);
    } 
}