namespace XeniaRegistrationBackend.Dtos
{
    public class CompanyCatalogRegistrationRequestDto
    {
        public string CompanyName { get; set; } = null!;
        public string CompanyAddress { get; set; } = null!;
        public string Phone1 { get; set; } = null!;
        public string Latitude { get; set; } = null!;
        public string Longitude { get; set; } = null!;
        public string Email { get; set; } = null!;

        public string? Phone2 { get; set; }
        public string? RegNo { get; set; }
        public string? AddressLine2 { get; set; }
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
        public string? Logo { get; set; }
        public string? SoldBy { get; set; }

        public string? UserName { get; set; }
        public string? Password { get; set; }

        public List<CompanyLabelDto> Labels { get; set; } = new();
        public List<CompanySettingDto> Settings { get; set; } = new();
    }
}