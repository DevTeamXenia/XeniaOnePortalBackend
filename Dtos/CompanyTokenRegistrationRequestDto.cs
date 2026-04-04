namespace XeniaRegistrationBackend.Dtos
{
    public class CompanyTokenRegistrationRequestDto
    {
        public string CompanyName { get; set; } = null!;
        public bool Status { get; set; }
        public string? Country { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }

        public CompanyTokenSettingsDto Settings { get; set; } = new();
    }

    public class CompanyTokenSettingsDto
    {
        public bool CollectCustomerName { get; set; }
        public bool PrintCustomerName { get; set; }
        public bool CollectCustomerMobileNumber { get; set; }
        public bool PrintCustomerMobileNumber { get; set; }
        public bool IsCustomCall { get; set; }
        public bool IsServiceEnable { get; set; }
    }
}
