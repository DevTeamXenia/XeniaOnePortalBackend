namespace XeniaRegistrationBackend.Dtos
{
    public class SubscribeTemplePlanRequestDto
    {
        public string PlanName { get; set; } = null!;
        public string? PlanDescription { get; set; }
        public int PlanUsers { get; set; }
        public bool planIsAddOn { get; set; }
        public bool PlanActive { get; set; } = true;
        public List<PlanDurationRequestDto> Durations { get; set; } = new();
    }

    public class PlanDurationRequestDto
    {
        public int DurationDays { get; set; }
        public decimal Price { get; set; }
    }

}
