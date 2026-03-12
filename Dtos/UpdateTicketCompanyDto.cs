namespace XeniaRegistrationBackend.Dtos
{
    public class UpdateTicketCompanyDto
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public List<CompanyTicketSettingDto> Settings { get; set; } = new();
        public List<CompanyTicketLabelDto> Labels { get; set; } = new();
    }
}
