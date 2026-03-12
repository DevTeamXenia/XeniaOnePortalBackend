namespace XeniaRegistrationBackend.Dtos
{
    public class SubscribeTemplePlanResponseDto
    {
        public int PlanId { get; set; }
        public int CompanyId { get; set; }
        public string PlanName { get; set; } = null!;
        public string? PlanDescription { get; set; }
        public int PlanUsers { get; set; }
        public bool PlanIsAddOn { get; set; }
        public bool PlanActive { get; set; }

        public List<PlanDurationResponseDto> Durations { get; set; } = new();
    }

    public class PlanDurationResponseDto
    {
        public int PlanDurationId { get; set; }
        public int DurationDays { get; set; }
        public decimal Price { get; set; }
    }

}
