namespace XeniaRegistrationBackend.Dtos
{
    public class SubscriptionHistoryDto
    {

        public int SubId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public int? DurationDays { get; internal set; }
        public string PlanName { get; internal set; }
    }
}
