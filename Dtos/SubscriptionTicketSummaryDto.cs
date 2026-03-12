namespace XeniaRegistrationBackend.Dtos
{
    public class SubscriptionTicketSummaryDto
    {
        public int SubId { get; set; }
        public string Status { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Amount { get; set; }
        public int UserCount { get; set; }
        public string PlanName { get; set; } = null!;
        public int PlanDuration { get; set; }
    }

}
