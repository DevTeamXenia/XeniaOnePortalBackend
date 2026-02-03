namespace XeniaRegistrationBackend.Dtos
{
    public class SubscribeTemplePlanResponseDto
    {
        public int PlanId { get; set; }
        public int CompanyId { get; set; }
        public string PlanName { get; set; } = null!;
        public string? PlanDescription { get; set; }
        public decimal PlanPrice { get; set; }
        public int PlanDurationDays { get; set; }
        public int PlanUsers { get; set; }
        public bool PlanIsAddOn { get; set; }
        public bool PlanActive { get; set; }
    }

}
