namespace XeniaRegistrationBackend.Dtos
{
    public class UpdateTempleCompanyDto
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string? Phone1 { get; set; }
        public string? Phone2 { get; set; }
        public string? RegNo { get; set; }
        public string? CompanyType { get; set; }
        public string? District { get; set; }
        public string? State { get; set; }
        public string? IFSCCode { get; set; }
        //public string? UserName { get; set; }
        //public string? Password { get; set; }

        public List<CompanySettingDto> Settings { get; set; } = new();
        public List<CompanyLabelDto> Labels { get; set; } = new();
    }

}
