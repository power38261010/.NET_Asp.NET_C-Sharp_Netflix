using System.ComponentModel.DataAnnotations;

namespace NetflixClone.Models
{
    public class MovieSubscription
    {
        [Key]
        public int Id { get; set; }

        // Clave foránea de User
        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        // Clave foránea de Subscription
        public int SubscriptionId { get; set; }
        public Subscription Subscription { get; set; }
    }
}
