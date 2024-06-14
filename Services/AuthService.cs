using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using NetflixClone.Data;
using NetflixClone.DTO;
using NetflixClone.Models;
using NetflixClone.Services.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NetflixClone.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;
        private readonly ApplicationDbContext _context;

        public AuthService(IConfiguration configuration, IMemoryCache cache, ApplicationDbContext context )
        {
            _configuration = configuration;
            _cache = cache;
            _context = context;
        }

        public string GenerateJwtToken( string Username, string Role) {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, Username),
                    new Claim(ClaimTypes.Role, Role),
                }),
                Expires = DateTime.UtcNow.AddDays(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]

            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var tokenString = tokenHandler.WriteToken(token);

            CacheToken(tokenString);
            return tokenString;
        }

        public bool ValidateJWTToken(string token)
        {
            if (string.IsNullOrEmpty(token) || !_cache.TryGetValue(token, out _))
            {
                return false;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"])),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return true;
            }
            catch (SecurityTokenException)
            {
                return false;
            }
        }

        public IEnumerable<Claim> GetUserClaimsFromToken (string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
            return jwtToken.Claims;
        }

        public Task Logout(string token)
        {
            RemoveCachedToken(token);
            return Task.CompletedTask;
        }

        public void CacheToken(string token)
        {
            _cache.Set(token, true, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(3)
            });
        }

        public void RemoveCachedToken(string token)
        {
            _cache.Remove(token);
        }
    }
}
