namespace XeniaRegistrationBackend.Dtos
{
    public class SubscriptionRentalSummaryDto
    {
        //    public int SubId { get; set; }
        //    public string Status { get; set; } = null!;
        //    public DateTime StartDate { get; set; }
        //    public DateTime EndDate { get; set; }
        //    public decimal Amount { get; set; }
        //    public string PlanName { get; set; } = null!;
        //    public int DurationDays { get; set; }
        //    public int UserCount { get; set; }  // ✅ ADD
        public int SubId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Amount { get; set; }
        public int? UserCount { get; set; }        // ✅ nullable
        public string PlanName { get; set; } = string.Empty;
        public int? DurationDays { get; set; }

        // ✅ Current subscription addons
        public List<SubscriptionRentalAddonDto>? Addons { get; set; }

        // ✅ Renewal history (all past subscriptions)
        public List<SubscriptionHistoryDto>? SubscriptionHistory
        {
            get; set;
        }
    }
}

