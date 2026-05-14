namespace XeniaRegistrationBackend.Dtos
{
    public class CompanyCatalogSubscriptionCreateDto
    {
        public int CompanyId { get; set; }
        public int CustomerId { get; set; }
        public int PlanId { get; set; }
        public int PlanDurationId { get; set; }
        public int? UserId { get; set; }
        public decimal? DealerAmount { get; set; }  // ← Add this

        public List<CatalogueAddonCreateDto>? Addons { get; set; }
    }
    public class CatalogueAddonCreateDto
    {
        public int PlanId { get; set; }
        public decimal? CustomAmount { get; set; }  // ONLY if you want override
        public int? UserCount { get; set; }
    }
}
