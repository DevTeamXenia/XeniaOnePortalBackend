namespace XeniaRegistrationBackend.Dtos
{
    public class SubscribeTemplePlanRequestDto
    {
        public string PlanName { get; set; } = null!;
        public string? PlanDescription { get; set; }
        public decimal PlanPrice { get; set; }
        public int PlanDurationDays { get; set; }
        public int PlanUsers { get; set; }
        public bool planIsAddOn { get; set; }
        public bool PlanActive { get; set; } = true;
    }

}
