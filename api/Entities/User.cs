namespace InventoryManagement.Api;

public enum UserRole
{
    Admin,
    User
}

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public UserRole Role { get; set; } = UserRole.User;

    public User(string username, string email, string password, UserRole role = UserRole.User)
    {
        Id = Guid.NewGuid();
        Username = username;
        Email = email;
        Password = password;
        Role = role;
    }

    public User()
    {
        
    }
}
