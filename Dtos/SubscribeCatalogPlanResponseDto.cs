namespace XeniaRegistrationBackend.Dtos
{
    public class SubscribeCatalogPlanResponseDto
    {
        public int PlanId { get; set; }
        public string PlanName { get; set; } = null!;
        public string? PlanDescription { get; set; }
        public int PlanUsers { get; set; }
        public bool PlanIsAddOn { get; set; }
        public bool PlanActive { get; set; }

        public List<CatalogPlanDurationResponseDto> Durations { get; set; } = new();
    }

    public class CatalogPlanDurationResponseDto
    {
        public int PlanDurationId { get; set; }
        public int DurationDays { get; set; }
        public decimal Price { get; set; }
        public decimal? DealerPrice { get; set; } 
        public bool IsActive { get; set; }
    }
}