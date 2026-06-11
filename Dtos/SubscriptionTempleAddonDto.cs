namespace XeniaRegistrationBackend.Dtos
{
    public class SubscriptionTempleAddonDto
    {
        public int SubAddonId { get; set; }
        public string SubAddonName { get; set; } = null!;
        public string? PlanDescription { get; set; }
        public decimal Amount { get; set; }
        public int UserCount { get; set; }
        public int PlanUsers { get; set; }
        public decimal? PlanPrice { get; set; }
        public bool PlanIsAddOn { get; set; }
        public bool PlanActive { get; set; }
    }

}
