using Microsoft.EntityFrameworkCore;
using NetflixClone.Data;
using NetflixClone.DTO;
using NetflixClone.Models;
using NetflixClone.Services.Contracts;
using System.Threading.Tasks;
using X.PagedList;
using System.Linq.Dynamic.Core;
using NetflixClone.Controllers.ModelRequest;

namespace NetflixClone.Services
{
    public class MovieService : IMovieService
    {
        private readonly ApplicationDbContext _context;

        public MovieService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Movie>> GetAllMovies()
        {
            return await _context.Movies
                                    .Include(m => m.MovieSubscription)
                                    .ToListAsync();
        }

        public async Task<Movie?> GetMovieById(int id)
        {
            try{
                // return await _context.Movies
                // .FindAsync(id);

                return await _context.Movies
                            .Include(m => m.MovieSubscription)
                            .FirstOrDefaultAsync(m => m.Id == id);
            }
            catch (Exception)
            {
                throw;
            }
        }

    public async Task<bool> CreateMovie(string? Title, string? Description, string? Genre, DateTime? ReleaseDate, string? PosterUrl, string? TrailerUrl, float? Rating, ICollection<MovieSubscription>? MovieSubscription)
    {
        try
        {

            var movie = new Movie
            {
                Title = Title,
                Description = Description,
                Genre = Genre,
                ReleaseDate = ReleaseDate ?? DateTime.Now,
                PosterUrl = PosterUrl,
                TrailerUrl = TrailerUrl,
                Rating = (float?)(Rating ?? 0)
            };

            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            if (MovieSubscription != null)
            {
                foreach (var subscription in MovieSubscription)
                {
                    _context.MovieSubscription.Add(new MovieSubscription
                    {
                        MovieId = movie.Id,
                        SubscriptionId = subscription.SubscriptionId
                    });
                }
                await _context.SaveChangesAsync();
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

        public async Task<bool> UpdateMovie(int Id, string? Title, string? Description, string? Genre, DateTime? ReleaseDate, string? PosterUrl, string? TrailerUrl, float? Rating, ICollection<MovieSubscription>? MovieSubscription)
        {
            var movie = await _context.Movies.Include(m => m.MovieSubscription)
                                            .FirstOrDefaultAsync(m => m.Id == Id);
            if (movie == null)
            {
                return false;
            }

            // Actualizar las propiedades de la película
            movie.Title = Title ?? movie.Title;
            movie.Description = Description ?? movie.Description;
            movie.Genre = Genre ?? movie.Genre;
            movie.ReleaseDate = ReleaseDate ?? movie.ReleaseDate;
            movie.PosterUrl = PosterUrl ?? movie.PosterUrl;
            movie.TrailerUrl = TrailerUrl ?? movie.TrailerUrl;
            movie.Rating = (float?)(Rating ?? movie.Rating);

            // Actualizar las relaciones de suscripción
            if (MovieSubscription != null)
            {
                var currentSubscriptions = movie.MovieSubscription.Select(ms => ms.SubscriptionId).ToList();
                var newSubscriptions = MovieSubscription.Select(ms => ms.SubscriptionId).ToList();

                // Eliminar suscripciones que ya no están en la lista
                foreach (var subscriptionId in currentSubscriptions.Except(newSubscriptions))
                {
                    var subscriptionToRemove = movie.MovieSubscription.FirstOrDefault(ms => ms.SubscriptionId == subscriptionId);
                    if (subscriptionToRemove != null)
                    {
                        _context.MovieSubscription.Remove(subscriptionToRemove);
                    }
                }

                // Agregar nuevas suscripciones
                foreach (var subscriptionId in newSubscriptions.Except(currentSubscriptions))
                {
                    movie.MovieSubscription.Add(new MovieSubscription
                    {
                        MovieId = movie.Id,
                        SubscriptionId = subscriptionId
                    });
                }
            }

            _context.Entry(movie).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }


    public async Task<IPagedList<MovieDto>> Search(
        string? title = null,
        string? description = null,
        string? genre = null,
        string operation = "==",
        DateTime? releaseDate = null,
        int? subscriptionType = null,
        string orderByProperty = "Title",
        int pageNumber = 1,
        int pageSize = 10)

    {
        var query = _context.Movies.Include(m => m.MovieSubscription).AsQueryable();

        // Apply filters based on provided parameters
        if (!string.IsNullOrEmpty(title))
        {
            query = query.Where(m => m.Title.Contains(title));
        }

        if (!string.IsNullOrEmpty(description))
        {
            query = query.Where(m => m.Description.Contains(description));
        }

        if (!string.IsNullOrEmpty(genre))
        {
            query = query.Where(m => m.Genre.Contains(genre));
        }

        if (releaseDate.HasValue)
        {
            switch (operation)
            {
                case ">=":
                    query = query.Where(m => m.ReleaseDate >= releaseDate);
                    break;
                case "<=":
                    query = query.Where(m => m.ReleaseDate <= releaseDate);
                    break;
                case "==":
                    query = query.Where(m => m.ReleaseDate == releaseDate);
                    break;
                case "!=":
                    query = query.Where(m => m.ReleaseDate != releaseDate);
                    break;
            }
        }

        // Sort by Title by default
        query = query.OrderBy(orderByProperty);

        // Return paginated results
        var movies = await query.ToPagedListAsync(pageNumber, pageSize);

        var movieDtos = movies.Select(movie => new MovieDto
        {
            Id = movie.Id,
            Title = movie.Title,
            Description = movie.Description,
            Genre = movie.Genre,
            ReleaseDate = movie.ReleaseDate,
            PosterUrl = movie.PosterUrl,
            TrailerUrl = movie.TrailerUrl,
            Rating = movie.Rating,
            MovieSubscriptions = movie.MovieSubscription?.Select(ms => new MovieSubscriptionDto
            {
                Id = ms.Id,
                MovieId = ms.MovieId,
                SubscriptionId = ms.SubscriptionId
            }).ToList()
        }).ToList();

        return new StaticPagedList<MovieDto>(movieDtos, pageNumber, pageSize, movies.TotalItemCount);
}


        public async Task<bool> LikeMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                movie.Rating += 0.01f;
                _context.Entry(movie).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> DislikeMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                movie.Rating -= 0.01f;
                _context.Entry(movie).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

    }

}
