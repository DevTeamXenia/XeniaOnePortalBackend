namespace XeniaRegistrationBackend.Dtos
{
    public class CompanyCatalogDetailDto
    {
        public CompanyCatalogListDto? Company { get; set; }  // ← ADD
        public List<CompanyCatalogSettingDto> Settings { get; set; } = new();
        public List<CompanyCatalogLabelDto> Labels { get; set; } = new();

    }
}