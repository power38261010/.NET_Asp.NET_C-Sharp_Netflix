using BussinessLayer.Services.Contracts;
using NetflixClone.DTO;
using NetflixClone.Models;
using X.PagedList;

namespace NetflixClone.Services.Contracts
{
    public interface IUserService /* : IBaseService <User>**/
    {
        Task<UserDto?> Register(User user);
        Task<UserDto?> Authenticate(string username, string password);
        Task<User?> GetUserById(int id);
        Task<UserDto?> GetUserDTO(User user);
        Task<User?> GetUserByUsername (string username);
        Task UpdateUser(User user);
        Task  DeleteUser(int id);
        Task<IPagedList<UserDto>> Search( string username , string role ,  DateTime? expirationDate , bool? isPaid ,  string subscriptionType ,   int pageIndex ,  int pageSize );
    }
}
