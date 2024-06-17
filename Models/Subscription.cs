using System;
using System.ComponentModel.DataAnnotations;

namespace NetflixClone.Models
{
    public class Subscription
    {
        [Key]
        public int Id { get; set; }
        public string? Type { get; set; } // maybe"standard" or "premium"

        public ICollection<MovieSubscription?> MovieSubscription { get; set; }

    }
}

