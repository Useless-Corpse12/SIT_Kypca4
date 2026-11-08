namespace VynilServer.Domain.Entities;

public class Product : Entity
{
    public string Genre { get; set; } = string.Empty;
    public string BandName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime ReleaseYear { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public ProductType Type { get; set; }
    public string UrlPhoto { get; set; } = string.Empty;

    public ICollection<Order> Orders { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
}

public enum ProductType
{
    Vinyl = 1,
    CD = 2,
    Audiocassette = 3
}