namespace XeniaRegistrationBackend.Dtos
{
    public class CompanyTicketDetailDto
    {
        public CompanyTicketListDto Company { get; set; } = null!;
        public List<CompanyTicketSettingDto> Settings { get; set; } = new();
        public List<CompanyTicketLabelDto> Labels { get; set; } = new();
    }
}
