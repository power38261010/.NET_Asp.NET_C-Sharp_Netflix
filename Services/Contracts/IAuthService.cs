using System.Security.Claims;
using BussinessLayer.Services.Contracts;
using NetflixClone.DTO;
using NetflixClone.Models;

namespace NetflixClone.Services.Contracts
{
    public interface IAuthService
    {
        string GenerateJwtToken(UserDto? user);
        bool ValidateJWTToken(string token);
        Task Logout(string token);
        void CacheToken(string token);
        void RemoveCachedToken(string token);
        IEnumerable<Claim> GetUserClaimsFromToken(string token);
    }
}
