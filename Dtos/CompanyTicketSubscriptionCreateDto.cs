namespace XeniaRegistrationBackend.Dtos
{
    public class CompanyTicketSubscriptionCreateDto
    {
        public int PlanId { get; set; }
        public int PlanDurationId { get; set; }
        public int CompanyId { get; set; }
        public List<CompanyTicketSubscriptionAddonCreateDto>? Addons { get; set; }

    }
}
