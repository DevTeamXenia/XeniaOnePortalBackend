namespace XeniaRegistrationBackend.Dtos
{
    public class UpdateTokenCompanyDto
    {
        public int CompanyID { get; set; }
        public string CompanyName { get; set; } = null!;

        public bool? Status { get; set; }

        public string? Country { get; set; }

        public string? Address { get; set; }

        public string? Email { get; set; }
        public List<CompanyTokenSettingsDto> Settings { get; set; } = new();
    }
}
