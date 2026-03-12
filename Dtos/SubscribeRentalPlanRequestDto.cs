namespace XeniaRegistrationBackend.Dtos
{
    public class SubscribeRentalPlanRequestDto
    {
        public string PlanName { get; set; } = null!;
        public string? PlanDescription { get; set; }
        public decimal PlanPrice { get; set; }
        public int PlanDurationDays { get; set; }
        public bool PlanActive { get; set; } = true;
    }
}
