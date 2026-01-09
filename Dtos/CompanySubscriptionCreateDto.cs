namespace XeniaRegistrationBackend.Dtos
{
    public class CompanySubscriptionCreateDto
    {
        public int PlanId { get; set; }
        public int CompanyId { get; set; }
        public List<CompanySubscriptionAddonCreateDto>? Addons { get; set; }
    }

}
