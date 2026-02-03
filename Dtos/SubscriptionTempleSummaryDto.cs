namespace XeniaRegistrationBackend.Dtos
{
    public class SubscriptionTempleSummaryDto
    {
        public int SubId { get; set; }
        public string Status { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Amount { get; set; }
        public int UserCount { get; set; }
        public string PlanName { get; set; } = null!;  
        public List<SubscriptionTempleAddonDto>? Addons { get; set; }
    }


}
