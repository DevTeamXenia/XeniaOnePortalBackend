namespace XeniaRegistrationBackend.Dtos
{
    public class CompanyCatalogListDto
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = null!;
        public string? CompanyAddress { get; set; }
        public string? CompanyPhone1 { get; set; }
        public string? CompanyPhone2 { get; set; }
        public string? CompanyRegNo { get; set; }
        public string? DistrictName { get; set; }
        public string? StateName { get; set; }
        public DateTime? CompanyCreatedOn { get; set; }  // ← nullable
        public bool? CompanyActive { get; set; }          // ← nullable
        public DateTime? CustomDate { get; set; }
        public SubscriptionCatalogueSummaryDto? Subscription { get; set; }
    }
}