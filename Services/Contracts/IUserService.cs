using BussinessLayer.Services.Contracts;
using NetflixClone.DTO;
using NetflixClone.Models;
using X.PagedList;

namespace NetflixClone.Services.Contracts
{
    public interface IUserService /* : IBaseService <User, UserDto>**/
    {
        Task<UserDto?> Register(string Username, string PasswordHash, string Email, int SubscriptionId );
        Task<UserDto?> Authenticate(string Username, string PasswordHash);
        Task<User?> GetUserById(int id);
        Task<UserDto?> GetUserDTO(User user);
        Task<User?> GetUserByUsername (string username);
        Task UpdateUser(int Id, string Username, string PasswordHash, string Email);
        Task UpdateRoleUser(int Id, string Role);
        Task  DeleteUser(int id);
        Task<IPagedList<UserDto>> Search( string username , string role ,  DateTime? expirationDate , bool? isPaid ,  string subscriptionType ,   int pageIndex ,  int pageSize );
    }
}
