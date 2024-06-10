using NetflixClone.Controllers.ModelRequest;
using NetflixClone.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using X.PagedList;

namespace NetflixClone.Services.Contracts
{
    public interface IMovieService
    {
        Task<IEnumerable<Movie>> GetAllMovies();
        Task<Movie?> GetMovieById(int id);
        Task<bool> CreateMovie(MovieRequest request);
        Task<bool> UpdateMovie(MovieRequest request);
        Task<bool> LikeMovie(int id);
        Task<bool> DislikeMovie(int id);

        Task<bool> DeleteMovie(int id);
        Task<IPagedList<MovieDto>> Search( string title , string description , string genre , string operation , DateTime? releaseDate , int? subscriptionType ,string orderByProperty , int pageIndex, int pageSize );
    }
}