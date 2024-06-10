using NetflixClone.Models;

namespace NetflixClone.DTO
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string? Role { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool? IsPaid { get; set; }
        public SubscriptionDTO? Subscription { get; set; }
        public int? SubscriptionId { get; set; }
    }

    public class SubscriptionDTO
    {
        public int Id { get; set; }
        public string? Type { get; set; }

        public SubscriptionDTO() { }

        public SubscriptionDTO(Subscription subscription)
        {
            Id = subscription.Id;
            Type = subscription.Type;
        }
    }
}
