namespace XeniaRegistrationBackend.Dtos
{
    public class CompanyTokenSubscriptionCreateDto
    {
        public int PlanId { get; set; }
        public int CompanyId { get; set; }
        public List<CompanyTempleSubscriptionAddonCreateDto>? Addons { get; set; }
    }

}
