namespace VynilServer.Domain.Entities;

public class Product : Entity
{
    public string Genre { get; set; } = string.Empty;
    public string BandName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public ProductType Type { get; set; }
    public string UrlPhoto { get; set; } = string.Empty;
    public ProductStatus Status { get; set; }
    public int Stock {get;set;} 

    public ICollection<OrderItem> Orders { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];

    public static Product Create(string genre, string bandName, string title, string description, int releaseYear,
        ProductType type, string urlPhoto)
    {
        return new Product
        {
            Genre = genre,
            BandName = bandName,
            Title = title,
            Description = description,
            ReleaseYear = releaseYear,
            Type = type,
            UrlPhoto = urlPhoto,
            Stock = 0,
            Cost = 0,
            Status = ProductStatus.Unavailable
        };

    }
    
    public void SetStock(int stock)
    {
        if (stock < 0)
            throw new ArgumentOutOfRangeException(nameof(stock), "Stock must be greater than or equal to 0.");
        if (Status != ProductStatus.Unavailable) Status = stock > 0 ? ProductStatus.Active : ProductStatus.Inactive;
    }

    public void ChangeStock(int value)
    {
        var newStock = Stock + value;
        if (newStock < 0)
            throw new ArgumentOutOfRangeException(nameof(value), "Stock must be greater than or equal to 0.");
        Stock = newStock;

        if (Status != ProductStatus.Unavailable)Status = Stock > 0 ? ProductStatus.Active : ProductStatus.Inactive;
        
    }
    
    public void SetCost(decimal cost)
    {
        if (cost <= 0)
            throw new ArgumentOutOfRangeException(nameof(cost), "Price must be greater than or equal to 0.");
        Cost =  cost;
    }
}

public enum ProductStatus
{
    Unavailable = 0,
    Active = 1,
    Inactive = 2
}

public enum ProductType
{
    Vinyl = 1,
    CD = 2,
    Audiocassette = 3
}