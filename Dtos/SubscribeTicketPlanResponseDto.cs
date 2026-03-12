namespace XeniaRegistrationBackend.Dtos
{
    public class SubscribeTicketPlanResponseDto
    {
        public int PlanId { get; set; }

        public string PlanName { get; set; } = null!;
        public string? PlanDescription { get; set; }
        public bool PlanIsAddOn { get; set; }
        public int PlanUsers { get; set; }
        public bool PlanActive { get; set; }

        public List<TicketPlanDurationDto> Durations { get; set; }
            = new();
    }

    public class TicketPlanDurationDto
    {
        public int PlanDurationId { get; set; }
        public int DurationDays { get; set; }
        public decimal Price { get; set; }
    }
}