using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetflixClone.Models
{
    public class MovieSubscription
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }

        // Clave foránea de User
        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        // Clave foránea de Subscription
        public int SubscriptionId { get; set; }
        public Subscription Subscription { get; set; }
    }
}
