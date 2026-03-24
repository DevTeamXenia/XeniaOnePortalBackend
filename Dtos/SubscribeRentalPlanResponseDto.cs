namespace XeniaRegistrationBackend.Dtos
{
    public class SubscribeRentalPlanResponseDto
    {
        public int PlanId { get; set; }

        public string PlanName { get; set; } = string.Empty;

        public string? PlanDescription { get; set; }

        public bool PlanActive { get; set; }
        public int? PlanUsers { get; set; }
        public bool PlanIsAddOn { get; set; }// change int to int?
        public List<SubscribeRentalPlanDurationDto> Durations { get; set; }
            = new();
    }

    public class SubscribeRentalPlanDurationDto
    {
        public int PlanDurationId { get; set; }
        public int DurationDays { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
    }
}