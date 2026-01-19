using XeniaRegistrationBackend.Dtos;

namespace XeniaRegistrationBackend.Repositories.CompanyRegistration
{
    public interface ICompanyRegistrationRepository
    {
        Task<int> RegisterCompanyAsync(CompanyRegistrationRequestDto request);
        Task<List<CompanyListDto>> GetAllTempleCompaniesAsync();
        Task<CompanyDetailDto?> GetCompanyByIdAsync(int companyId);
        Task UpdateCompanyAsync(UpdateCompanyDto dto);

     //   Task<List<CompanyListDto>> GetAllTokenCompaniesAsync();
    }
}
