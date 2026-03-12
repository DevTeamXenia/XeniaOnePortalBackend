namespace XeniaRegistrationBackend.Dtos
{
    public class CompanyRentalSubscriptionCreateDto
    {
        public int PlanId { get; set; }
        public int PlanDurationId { get; set; }
        public int CompanyId { get; set; }

        public List<CompanyRentalSubscriptionAddonCreateDto>? Addons { get; set; }
    }
}
