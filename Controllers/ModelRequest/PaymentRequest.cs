using System.ComponentModel.DataAnnotations.Schema;
using NetflixClone.Models;

public class PaymentRequest
    {
        public decimal Amount { get; set; }
        public string Token { get; set; }
        public string PayerEmail { get; set; }
        public string Description { get; set; }
        public string PaymentMethodId { get; set; }
        public bool IsAnual { get; set; }

        [ForeignKey("PayId")]
        public int PayId { get; set; }
        public Pay? Pay { get; set; }
    }