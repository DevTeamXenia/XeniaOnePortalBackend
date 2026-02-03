namespace XeniaRegistrationBackend.Dtos
{
    public class SubscriptionTokenAddonDto
    {
        public int SubAddonId { get; set; }
        public string SubAddonName { get; set; } = null!;
        public decimal Amount { get; set; }
        public int DepCount { get; set; }
    }
}
