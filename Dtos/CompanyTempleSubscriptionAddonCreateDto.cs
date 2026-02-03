namespace XeniaRegistrationBackend.Dtos
{
    public class CompanyTempleSubscriptionAddonCreateDto
    {
        public int MainPlanId { get; set; }
        public int PlanId { get; set; }
        public int CompanyId { get; set; }
        public decimal Amount { get; set; }
        public int UserCount { get; set; }
        public string Status { get; set; }
    }

}
