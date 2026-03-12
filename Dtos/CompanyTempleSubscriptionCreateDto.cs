namespace XeniaRegistrationBackend.Dtos
{
    public class CompanyTempleSubscriptionCreateDto
    {
        public int PlanId { get; set; }
        public int CompanyId { get; set; }
        public int PlanDurationId { get; set; }
        public List<CompanyTempleSubscriptionAddonCreateDto>? Addons { get; set; }
    }

}
