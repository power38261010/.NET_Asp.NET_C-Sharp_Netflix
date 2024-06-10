using Microsoft.EntityFrameworkCore;
using NetflixClone.Data;
using NetflixClone.Models;
using NetflixClone.Services.Contracts;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;
using System.Linq;
using NetflixClone.DTO;
using NetflixClone.Services;

namespace NetflixClone.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        private static string GenerateHash(string password)
        {
            // Reemplazar con BCrypt o Argon2 para un hashing seguro
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashBytes);
        }

        private  bool ValidatePassword(string passwordHash, string password)
        {
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            var computedHash = Convert.ToBase64String(hashBytes);

            return passwordHash == computedHash;
        }

        public async Task<User?> GetUserByUsername(string username)
        {
            return await _context.Users
                            .Include(u => u.Subscription)
                            .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<UserDto?> Register(User user)
        {
            if (await _context.Users.AnyAsync(u => u.Username == user.Username))
            {
                throw new Exception("Username already exists");
            }

            user.PasswordHash = GenerateHash(user.PasswordHash);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return await GetUserDTO (user);
        }

        public async Task<UserDto?> Authenticate(string username, string password)
        {

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user != null && ValidatePassword(user.PasswordHash, password))
            {
                return await GetUserDTO (user);
            }

            return null;
        }

        public async Task<User?> GetUserById(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<UserDto?> GetUserDTO(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role,
                ExpirationDate = user.ExpirationDate,
                IsPaid = user.IsPaid,
                Subscription = user.SubscriptionId != null ? new SubscriptionDTO(await _context.Subscriptions.FindAsync((int)user.SubscriptionId)) : null
            };
        }

        public async Task UpdateUser(User user)
        {
            // Generar nuevo hash de contraseña si se modifica
            if (!string.IsNullOrEmpty(user.PasswordHash))
            {
                user.PasswordHash = GenerateHash(user.PasswordHash);
            }

            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IPagedList<UserDto>> Search(
            string? username = null,
            string? role = null,
            DateTime? expirationDate = null,
            bool? isPaid = null,
            string? subscriptionType = null,
            int pageIndex = 1,
            int pageSize = 10)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(username))
            {
                query = query.Where(u => u.Username.Contains(username));
            }

            if (!string.IsNullOrEmpty(role))
            {
                query = query.Where(u => u.Role.Contains(role));
            }

            if (expirationDate.HasValue)
            {
                query = query.Where(u => u.ExpirationDate >= expirationDate);
            }

            if (isPaid.HasValue)
            {
                query = query.Where(u => u.IsPaid == isPaid);
            }

            if (!string.IsNullOrEmpty(subscriptionType))
            {
                query = query.Where(u => u.Subscription.Type.Contains(subscriptionType));
            }

            query = query.OrderBy(u => u.Username);
            var users = await query.Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Role = u.Role,
                ExpirationDate = u.ExpirationDate,
                IsPaid = u.IsPaid,
                SubscriptionId =  (int) u.SubscriptionId
            }).ToPagedListAsync(pageIndex, pageSize);

            return users;
        }

    }
}
