namespace XeniaRegistrationBackend.Dtos
{
    public class CompanyTempleDetailDto
    {
        public CompanyTempleListDto Company { get; set; } = null!;
        public List<CompanySettingDto> Settings { get; set; } = new();
        public List<CompanyLabelDto> Labels { get; set; } = new();
    }

}
