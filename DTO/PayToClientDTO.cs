

using NetflixClone.Models;

namespace NetflixClone.DTO;
public class PayDto
{
        public int Id { get; set; }
        public required string Currency { get; set; }
        public decimal MonthlyPayment { get; set; }
        public decimal AnnualMultiplierPayment { get; set; }
        public decimal InterestMonthlyPayment { get; set; }

        public int? SubscriptionId { get; set; }
        public Subscription? Subscription { get; set; }
}


