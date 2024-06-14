using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetflixClone.Models;
using NetflixClone.Services;
using NetflixClone.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NetflixClone.Controllers.ModelRequest;

namespace NetflixClone.Controllers
{
    [Route("api/movies")]
    [ApiController]
    public class MoviesController : Controller
    {
        private readonly IMovieService _movieService;
        private readonly IUserService _userService;
        private readonly ILogger<MoviesController> _logger;

        public MoviesController(IMovieService movieService, IUserService userService, ILogger<MoviesController> logger)
        {
            _movieService = movieService;
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllMovies()
        {
            try {
                var movies = await _movieService.GetAllMovies();
                return Ok(movies);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error durante el uso de _movieService.GetAllMovies: {ex.Message}");
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchMovies(
            string? title = null,
            string? description = null,
            string? genre = null,
            string operation = "==",
            DateTime? releaseDate = null,
            int? subscriptionType = null,
            string orderByProperty = "Title",
            int pageIndex = 1,
            int pageSize = 10)
        {
            try
            {

                var username = HttpContext.User.Identity?.Name;
                var userSesion = (User?) await _userService.GetUserByUsername(username);

                if ( userSesion != null) {
                    if ( userSesion.Role != "admin") {
                        subscriptionType = userSesion.SubscriptionId;
                    }
                    var movies = await _movieService.Search(title, description, genre, operation, releaseDate, subscriptionType, orderByProperty, pageIndex, pageSize);
                    return Ok(movies);
                }
                return BadRequest(new { Message = "Usuario no autenticado" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error durante el uso _movieService.Search: {ex.Message}");
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetMovieById(int id)
        {
            try {
                var movie = await _movieService.GetMovieById(id);
                if (movie == null)
                {
                    return NotFound();
                }
                return Ok(movie);
            }
            catch (Exception ex)
            {
                // Loguea cualquier excepción que ocurra durante el inicio de sesión
                _logger.LogError($"Error durante el uso de _movieService.GetMovieById: {ex.Message}");
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> CreateMovie([FromBody] MovieRequest request)
        {
            try
            {
                var success = await _movieService.CreateMovie(request.Title, request.Description, request.Genre, request.ReleaseDate, request.PosterUrl, request.TrailerUrl, (float?)request.Rating, (ICollection<MovieSubscription>?)request.MovieSubscriptionRequest );
                if (success)
                {
                    return CreatedAtAction(nameof(GetMovieById), new { id = request.Id }, request);
                }
                return BadRequest(new { Message = "Unable to create movie" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> UpdateMovie(int id, [FromBody] MovieRequest request)
        {
            try {
                if (id != request.Id)
                {
                    return BadRequest();
                }

                var success = await _movieService.UpdateMovie( id, request.Title, request.Description, request.Genre, request.ReleaseDate, request.PosterUrl, request.TrailerUrl, (float?) request.Rating, (ICollection<MovieSubscription>?)request.MovieSubscriptionRequest );
                if (success)
                {
                    var updatedMovie = await _movieService.GetMovieById(id);
                    return Ok(updatedMovie);
                }
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating movie");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error durante el uso de _movieService.UpdateMovie: {ex.Message}");
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<ActionResult> DeleteMovie(int id)
        {
            try {
                var existingMovie = await _movieService.GetMovieById(id);
                if (existingMovie == null)
                {
                    return NotFound();
                }

                await _movieService.DeleteMovie(id);
                return Ok(new {Message = "Movie Deleted"});
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error durante el uso de _movieService.GetMovieById: {ex.Message}");
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut("rating/{id}/{isLike}")]
        public async Task<IActionResult> RatingMovie(int id, bool isLike)
        {
            var success = false;
            try {
                if (isLike) {
                success = await _movieService.LikeMovie( id);
                } else if (!isLike) {
                success = await _movieService.DislikeMovie( id);
                }
                if (success)
                {
                    return Ok(new {Message = "Movie Rated"});
                }
                return StatusCode(StatusCodes.Status500InternalServerError, "Error rating movie");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error durante el uso de RatingMovie: {ex.Message}");
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
