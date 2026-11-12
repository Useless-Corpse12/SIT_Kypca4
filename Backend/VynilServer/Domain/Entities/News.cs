namespace VynilServer.Domain.Entities;

public class News : Entity
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string PhotoUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    private News() { }

    public static News Create(string title, string content, string photoUrl)
    {
        return new News
        {
            Id = Guid.NewGuid(),
            Title = title,
            Content = content,
            PhotoUrl = $"photos/{photoUrl}",
            CreatedAt = DateTime.UtcNow
        };
    }
}