namespace XeniaRegistrationBackend.Dtos
{
    public class CompanySubscriptionAddonCreateDto
    {
        public int PlanId { get; set; }
        public int CompanyId { get; set; }
        public decimal Amount { get; set; }
        public int UserCount { get; set; }
    }

}
