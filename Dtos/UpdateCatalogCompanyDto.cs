namespace XeniaRegistrationBackend.Dtos
{
    public class UpdateCatalogCompanyDto
    {
        public int CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyAddress { get; set; }
        public string? AddressLine2 { get; set; }
        public string? Phone1 { get; set; }
        public string? Phone2 { get; set; }
        public string? Email { get; set; }
        public string? RegNo { get; set; }
        public string? Pincode { get; set; }
        public string? Website { get; set; }
        public string? Logo { get; set; }
        public string? TaxType { get; set; }
        public string? Currency { get; set; }
        public string? CurrencySymbol { get; set; }
        public string? MajorCurrency { get; set; }
        public string? MinorCurrency { get; set; }
        public string? OrderNoPrefix { get; set; }
        public string? OrderNoSuffix { get; set; }
        public int? BranchLimit { get; set; }
        public string? CompanyBrand { get; set; }
        public string? CustomerCareNo { get; set; }
        public string? CustomerCareEmail { get; set; }
        public string? FooterMessage { get; set; }
        public string? SoldBy { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public int? DecimalValue { get; set; }
        public bool? CompanyActive { get; set; }
        public List<CompanyCatalogSettingDto> Settings { get; set; } = new();
        public List<CompanyCatalogLabelDto> Labels { get; set; } = new();
    }
}