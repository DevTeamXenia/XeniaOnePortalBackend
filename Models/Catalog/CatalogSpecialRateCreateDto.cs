namespace XeniaRegistrationBackend.Models.Catalog
{
    public class CatalogSpecialRateCreateDto
    {
        public int? CustomerId { get; set; }
        public int CompanyId { get; set; }
        public int PlanId { get; set; }
        public int PlanDurationId { get; set; }
        public int? AddonId { get; set; }
        public decimal CustomRate { get; set; }  // ← SIMPLE CUSTOM RATE
        public int? UserId { get; set; }
    }

    public class CatalogSpecialRateResponseDto
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public int CompanyId { get; set; }
        public int PlanId { get; set; }
        public int PlanDurationId { get; set; }
        public int? AddonId { get; set; }
        public decimal? CustomRate { get; set; }  // ← SIMPLE CUSTOM RATE
        public int? UserId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
