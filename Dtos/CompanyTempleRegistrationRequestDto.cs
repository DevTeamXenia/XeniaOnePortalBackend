namespace XeniaRegistrationBackend.Dtos
{
    public class CompanyTempleRegistrationRequestDto
    {
        public string CompanyName { get; set; } = null!;
        public string? CompanyAddress { get; set; }
        public string? Phone1 { get; set; }
        public string? Phone2 { get; set; }
        public string? RegNo { get; set; }
        public string? CompanyType { get; set; }
        public string? District { get; set; }
        public string? State { get; set; }
        public string? IFSCCode { get; set; }

        public List<CompanyLabelDto> Labels { get; set; } = new();
        public List<CompanySettingDto> Settings { get; set; } = new();
    }

    public class CompanyLabelDto
    {
        public string SettingKey { get; set; } = null!;
        public string? DisplayName { get; set; }
        public string? DisplayNameTa { get; set; }
        public string? DisplayNameMa { get; set; }
    }

    public class CompanySettingDto
    {
        public string KeyCode { get; set; } = null!;
        public string Value { get; set; } = null!;
    }

}
