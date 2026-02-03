namespace XeniaRegistrationBackend.Dtos
{
    public class SubscriptionTokenSummaryDto
    {
        public int SubId { get; set; }
        public string Status { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Amount { get; set; }
        public int DepCount { get; set; }
        public string PlanName { get; set; } = null!;
        public List<SubscriptionTokenAddonDto>? Addons { get; set; }
    }
}
