public class SubscriptionRequest {
    public int Id { get; set; }
    public string Type { get; set; }
    public ICollection<MovieSubscriptionRequest>? MovieSubscriptionRequest { get; set; } = new List<MovieSubscriptionRequest>();

}