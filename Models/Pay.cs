using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetflixClone.Models
{
    public class Pay
    {
        [Key]
        public int Id { get; set; }
        public string Currency { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal MonthlyPayment { get; set; }

        [ForeignKey("SubscriptionId")]
        public int? SubscriptionId { get; set; }
        public Subscription? Subscription { get; set; }
    }
}


