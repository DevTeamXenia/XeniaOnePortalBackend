namespace XeniaRegistrationBackend.Dtos
{
    public class SubscriptionTempleAddonDto
    {
        public int SubAddonId { get; set; }
        public string SubAddonName { get; set; } = null!;
        public decimal Amount { get; set; }
        public int UserCount { get; set; }
    }

}
