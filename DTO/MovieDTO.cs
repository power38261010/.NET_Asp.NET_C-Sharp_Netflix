using NetflixClone.Models;
namespace NetflixClone.DTO;
public class MovieDto
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Genre { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public string? PosterUrl { get; set; }
    public string? TrailerUrl { get; set; }
    public float? Rating { get; set; }
    public List<MovieSubscriptionDto>? MovieSubscriptions { get; set; }
}

public class MovieSubscriptionDto
{
    public int Id { get; set; }
    public int? SubscriptionId { get; set; }
    public Subscription? Subscription { get; set; }
}
