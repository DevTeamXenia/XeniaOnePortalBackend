namespace XeniaRegistrationBackend.Dtos
{
    public class CompanyDetailDto
    {
        public CompanyListDto Company { get; set; } = null!;
        public List<CompanySettingDto> Settings { get; set; } = new();
        public List<CompanyLabelDto> Labels { get; set; } = new();
    }

}
