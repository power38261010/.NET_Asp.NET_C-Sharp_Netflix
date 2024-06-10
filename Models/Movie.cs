using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
namespace NetflixClone.Models
{
    public class Movie
    {
        [Key]

        public int Id { get; set; }
        public string? Title { get; set; }
        [Column(TypeName = "text")]
        public string? Description { get; set; }
        public string? Genre { get; set; }
        public DateTime? ReleaseDate { get; set; }
        [Column(TypeName = "text")]
        public string? PosterUrl { get; set; }
        [Column(TypeName = "text")]
        public string? TrailerUrl { get; set; }
        public float? Rating { get; set; }

        public ICollection<MovieSubscription>? MovieSubscription { get; set; }
    }
}