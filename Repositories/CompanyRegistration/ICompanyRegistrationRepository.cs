using XeniaRegistrationBackend.Dtos;

namespace XeniaRegistrationBackend.Repositories.CompanyRegistration
{
    public interface ICompanyRegistrationRepository
    {
        Task<int> RegisterTempleCompanyAsync(CompanyTempleRegistrationRequestDto request);
        Task<List<CompanyTempleListDto>> GetAllTempleCompaniesAsync();
        Task<CompanyTempleDetailDto?> GetTempleCompanyByIdAsync(int companyId);
        Task UpdateTempleCompanyAsync(UpdateTempleCompanyDto dto);


        Task<int> RegisterTokenCompanyAsync(CompanyTokenRegistrationRequestDto request);
        Task<List<CompanyTokenListDto>> GetAllTokenCompaniesAsync();
        Task<CompanyTokenDetailDto?> GetTokenCompanyByIdAsync(int companyId);
        Task UpdateTokenCompanyAsync(UpdateTokenCompanyDto dto);



        Task<int> RegisterRentalCompanyAsync(CompanyRentalRegistrationRequestDto request);
        Task<List<CompanyRentalListDto>> GetAllRentalCompaniesAsync();
        Task<CompanyRentalDetailDto?> GetRentalCompanyByIdAsync(int companyId);
        Task UpdateRentalCompanyAsync(UpdateRentalCompanyDto dto);


        Task<int> RegisterTicketCompanyAsync(CompanyTicketRegistrationRequestDto request);
        Task<List<CompanyTicketListDto>> GetAllTicketCompaniesAsync();
        Task<CompanyTicketDetailDto?> GetTicketCompanyByIdAsync(int companyId);
        Task UpdateTicketCompanyAsync(UpdateTicketCompanyDto dto);
    }
}
