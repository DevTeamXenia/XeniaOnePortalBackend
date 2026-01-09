namespace XeniaRegistrationBackend.Dtos
{
    public class SubscriptionSummaryDto
    {
        public int SubId { get; set; }
        public string Status { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Amount { get; set; }
        public int UserCount { get; set; }

        public List<SubscriptionAddonDto>? Addons { get; set; }
    }

}
