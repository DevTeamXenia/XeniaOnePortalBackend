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
        public DateTime? CompanyCreatedOn { get; set; }
        public string? CompanyLogo { get; set; }
        public bool? CompanyActive { get; set; }
        
        // newly added fields
        public string? AddressLine2 { get; set; }
        public string? Email { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string? Pincode { get; set; }
        public string? Website { get; set; }
        public int? DecimalValue { get; set; }
        public string? TaxType { get; set; }
        public string? Currency { get; set; }
        public string? CurrencySymbol { get; set; }
        public string? MajorCurrency { get; set; }
        public string? MinorCurrency { get; set; }
        public string? OrderNoPrefix { get; set; }
        public string? OrderNoSuffix { get; set; }
        public string? BranchLimit { get; set; }
        public string? CompanyBrand { get; set; }
        public string? CustomerCareNo { get; set; }
        public string? CustomerCareEmail { get; set; }
        public string? FooterMessage { get; set; }
        public string? SoldBy { get; set; }

        public SubscriptionCatalogueSummaryDto? Subscription { get; set; }
    }
}