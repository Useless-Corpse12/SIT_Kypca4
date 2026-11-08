namespace VynilServer.Domain.Entities;

public class User : Entity
{
    public string Login { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public Role Role { get; set; }
    public bool IsVerified { get; set; } = false;

    public ICollection<Order> Purchases { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
    
    private User() { }

    public static User Create(string login, string email, string passwordHash)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Login = login,
            Email = email.ToLower(),
            PasswordHash = passwordHash,
            Role = Role.User
        };
    }

    public static User CreateAdmin(string login, string email, string passwordHash)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Login = login,
            Email = email.ToLower(),
            PasswordHash = passwordHash,
            Role = Role.Admin
        };
    }

    public void VerifyEmail()
    {
        IsVerified = true;
    }
}

public enum Role
{
    User = 1,
    Admin = 2
}