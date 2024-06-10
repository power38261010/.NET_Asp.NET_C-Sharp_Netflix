using NetflixClone.Models;

namespace NetflixClone.Controllers.ModelRequest;

public class AuthRequest
{
    public string Username { get; set; }
    public string PasswordHash { get; set; }
}
