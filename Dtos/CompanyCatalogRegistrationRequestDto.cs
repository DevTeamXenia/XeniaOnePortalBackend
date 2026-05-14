namespace XeniaRegistrationBackend.Dtos
{
    public class CompanyCatalogRegistrationRequestDto
    {
        public string CompanyName { get; set; } = null!;
        public string? CompanyAddress { get; set; }
        public string? Phone1 { get; set; }
        public string? Phone2 { get; set; }
        public string? RegNo { get; set; }
        public string? Email { get; set; }        // ✅ Add this
        public string? CompanyType { get; set; }
        public string? District { get; set; }
        public string? State { get; set; }
        public string? IFSCCode { get; set; }

        public string? UserName { get; set; }
        public string? Password { get; set; }

        // ✅ YOUR CUSTOM CHANGE
        public DateTime? CustomDate { get; set; }

        public List<CompanyLabelDto> Labels { get; set; } = new();
        public List<CompanySettingDto> Settings { get; set; } = new();
    }
}