using NetflixClone.Models;

namespace NetflixClone.Controllers.ModelRequest;

public class UserRequest
{
    public int Id { get; set; }
    public string? Username { get; set; }
    public string? PasswordHash { get; set; }
    public string? PasswordHashNew { get; set; }
    public string? Email { get; set; }
    public int? SubscriptionId { get; set; }

}
