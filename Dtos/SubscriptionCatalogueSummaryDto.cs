namespace XeniaRegistrationBackend.Dtos
{
    public class SubscriptionCatalogueSummaryDto
    {
        public int SubId { get; set; }
        public string? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? Amount { get; set; }
        public decimal? DealerAmount { get; set; }
        //public decimal? CustomAmount { get; set; }
        public string? RateType { get; set; }
        public int UserCount { get; set; }
        public string? PlanName { get; set; }
        public int? DurationDays { get; set; }
        public List<CatalogueAddonSummaryDto>? Addons { get; set; }
    }
    public class CatalogueAddonSummaryDto
    {
        public int SubAddonId { get; set; }
        public string? AddonPlanName { get; set; }
        public decimal? Amount { get; set; }
        public decimal? DealerAmount { get; set; }
        //public decimal? CustomAmount { get; set; }
        public string? RateType { get; set; }
        public int? UserCount { get; set; }
        public string? Status { get; set; }
    }
}
