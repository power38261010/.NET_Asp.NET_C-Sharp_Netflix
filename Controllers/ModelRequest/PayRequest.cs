using NetflixClone.Models;

namespace NetflixClone.Controllers.ModelRequest;

public class PayRequest
{
        public int? Id { get; set; }
        public string Currency { get; set; }
        public decimal MonthlyPayment { get; set; }
        public int? SubscriptionId { get; set; }
        public Subscription? Subscription{ get; set; }
}
