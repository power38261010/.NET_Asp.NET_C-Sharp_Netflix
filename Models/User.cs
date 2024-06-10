using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetflixClone.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string? Role { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool? IsPaid { get; set; }

        [ForeignKey("SubscriptionId")]
        public int? SubscriptionId { get; set; }
        public Subscription? Subscription { get; set; }
    }

}
