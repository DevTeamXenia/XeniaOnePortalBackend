namespace XeniaRegistrationBackend.Dtos
{
    public class SubscribeTemplePlanRequestDto
    {
        public string PlanName { get; set; } = null!;
        public string? PlanDescription { get; set; }
        public int PlanUsers { get; set; }

        public bool PlanIsAddOn { get; set; }

        public bool PlanActive { get; set; } = true;

        // Used only for Add-On
        public decimal? PlanPrice { get; set; }

        // Used only for Normal Plan
        public List<PlanDurationRequestDto>? Durations { get; set; }

    }

    public class PlanDurationRequestDto
    {
        public int DurationDays { get; set; }
        public decimal Price { get; set; }
    }

    public class AddonPriceDto
    {
        public decimal Price { get; set; }
    }
    

   

}
