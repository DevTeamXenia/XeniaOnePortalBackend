namespace XeniaRegistrationBackend.Dtos
{
    public class CompanyRentalDetailDto
    {
        public CompanyRentalListDto Company { get; set; } = null!;
        public List<CompanyRentalSettingsDto> Settings { get; set; } = new();
    }

}
