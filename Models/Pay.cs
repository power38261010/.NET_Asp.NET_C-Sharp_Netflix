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
        public float MonthlyPayment { get; set; }

        [ForeignKey("SubscriptionId")]
        public int? SubscriptionId { get; set; }
        public Subscription? Subscription { get; set; }
    }
}


