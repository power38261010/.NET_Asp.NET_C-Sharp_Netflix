

namespace NetflixClone.DTO;
public class PayDto
{
        public int Id { get; set; }
        public required string Currency { get; set; }
        public float MonthlyPayment { get; set; }
        public float AnnualMultiplierPayment { get; set; }
        public float InterestMonthlyPayment { get; set; }

        public int? SubscriptionId { get; set; }
}


