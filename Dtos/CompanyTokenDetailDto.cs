namespace XeniaRegistrationBackend.Dtos
{
   public class CompanyTokenDetailDto
{
    public CompanyTokenListDto Company { get; set; } = null!;
    public CompanyTokenSettingsDto Settings { get; set; } = new();
}

}
