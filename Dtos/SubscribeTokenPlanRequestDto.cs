namespace XeniaRegistrationBackend.Dtos
{
    public class SubscribeTokenPlanRequestDto
    {
        public string PlanName { get; set; } = null!;
        public string? PlanDescription { get; set; }
        public int PlanDeps { get; set; }
        public int? PlanUsers { get; set; }
        public bool planIsAddOn { get; set; }
        public bool PlanActive { get; set; } = true;

        public List<SubscribeTokenPlanDurationDto> Durations { get; set; }
            = new();
    }

    public class SubscribeTokenPlanDurationDto
    {
        public int DurationDays { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
    }

}
