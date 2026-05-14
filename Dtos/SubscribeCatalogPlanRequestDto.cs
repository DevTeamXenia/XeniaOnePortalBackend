namespace XeniaRegistrationBackend.Dtos
{
    public class SubscribeCataloguePlanRequestDto
    {
        public string? PlanName { get; set; }
        public string? PlanDescription { get; set; }
        public int? PlanUsers { get; set; }
        public bool PlanIsAddOn { get; set; }
        public bool PlanActive { get; set; }
        public List<CataloguePlanDurationDto> Durations { get; set; } = new();
    }

    public class CatalogPlanDurationRequestDto
    {
        public int DurationDays { get; set; }
        public decimal Price { get; set; }          // Standard Rate
        public decimal? DealerPrice { get; set; }   // Dealer Rate
   /*     public decimal? CustomPrice { get; set; }   */// Custom Rate
    }
    public class CataloguePlanDurationDto
    {
        public int DurationDays { get; set; }
        public decimal Price { get; set; }
        public decimal? DealerPrice { get; set; }
        //public decimal? CustomPrice { get; set; }
    }

    public class SubscribeCataloguePlanResponseDto
    {
        public int PlanId { get; set; }
        public string? PlanName { get; set; }
        public string? PlanDescription { get; set; }
        public int? PlanUsers { get; set; }
        public bool? PlanIsAddOn { get; set; }
        public bool? PlanActive { get; set; }
        public List<CataloguePlanDurationResponseDto> Durations { get; set; } = new();
    }

    public class CataloguePlanDurationResponseDto
    {
        public int PlanDurationId { get; set; }
        public int? DurationDays { get; set; }
        public decimal? Price { get; set; }
        public decimal? DealerPrice { get; set; }
        //public decimal? CustomPrice { get; set; }
    }
}
