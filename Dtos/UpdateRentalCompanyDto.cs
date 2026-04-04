namespace XeniaRegistrationBackend.Dtos
{
    public class UpdateRentalCompanyDto
    {

        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = null!;

        public bool? Status { get; set; }

        public string? Country { get; set; }

        public string? Address { get; set; }

        public string? Email { get; set; }
        //public string? UserName { get; set; }
        //public string? Password { get; set; }

        public List<CompanyRentalSettingsDto> Settings { get; set; } = new();
    }
}
