using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetflixClone.Models {
    public class PaySubscription {
        [Key]
        public int Id { get; set; }
        public bool IsAnual { get; set; }
        public string PayerEmail { get; set; }
        public string Token { get; set; }
        public string Description { get; set; }
        public string status { get; set; }
        public DateTime PaidDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [ForeignKey("PayId")]
        public int PayId { get; set; }
        public Pay? Pay { get; set; }
        [ForeignKey("UserId")]
        public int UserId { get; set; }
        public User? User { get; set; }
    }

}
