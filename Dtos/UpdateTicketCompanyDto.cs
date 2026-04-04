namespace XeniaRegistrationBackend.Dtos
{
    public class UpdateTicketCompanyDto
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = null!;
        public string Address { get; set; } = null!;
        //public string? UserName { get; set; }
        //public string? Password { get; set; }

        public List<CompanyTicketSettingDto> Settings { get; set; } = new();
        public List<CompanyTicketLabelDto> Labels { get; set; } = new();
    }
}
