namespace XeniaRegistrationBackend.Dtos
{
    public class UpdateCompanyDto
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public List<CompanySettingDto> Settings { get; set; } = new();
        public List<CompanyLabelDto> Labels { get; set; } = new();
    }

}
