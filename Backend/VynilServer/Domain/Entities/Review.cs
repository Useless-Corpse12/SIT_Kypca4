namespace VynilServer.Domain.Entities;

public class Review: Entity
{
    public User User { get; set; } = null!;
    public Product Product { get; set; } = null!;
    public string Body { get; set; } = string.Empty;
    public byte Rate { get; set; } // 1-10
}