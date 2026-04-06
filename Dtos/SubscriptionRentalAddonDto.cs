namespace XeniaRegistrationBackend.Dtos
{
    public class SubscriptionRentalAddonDto
    {
        public int SubAddonId { get; set; }
        public string AddonPlanName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int UserCount { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}