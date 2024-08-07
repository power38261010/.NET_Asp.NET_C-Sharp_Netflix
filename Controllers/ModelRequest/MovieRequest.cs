using NetflixClone.Models;

namespace NetflixClone.Controllers.ModelRequest;
public class MovieRequest {
    public int? Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Genre { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public string? PosterUrl { get; set; }
    public string? TrailerUrl { get; set; }
    public double? Rating { get; set; }

    public ICollection<MovieSubscriptionRequest>? MovieSubscriptions { get; set; }
}

