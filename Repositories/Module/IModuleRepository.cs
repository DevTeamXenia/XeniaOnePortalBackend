using XeniaRegistrationBackend.Models;
using XeniaRegistrationBackend.Models.Catalog;
using XeniaRegistrationBackend.Models.Temple;

namespace XeniaRegistrationBackend.Repositories.Module
{
    public interface IModuleRepository
    {
        // Temple
        Task<int> CreateTempleModuleAsync(TK_Module request);
        Task<bool> UpdateTempleModuleAsync(int moduleId, TK_Module request);
        Task<IEnumerable<TK_Module>> GetAllTempleModuleAsync();
        Task<TK_Module?> GetByIdTempleModuleAsync(int moduleId);

        // Rental
        Task<int> CreateRentalModuleAsync(XRS_Module request);
        Task<bool> UpdateRentalModuleAsync(int moduleId, XRS_Module request);
        Task<IEnumerable<XRS_Module>> GetAllRentalModuleAsync();
        Task<XRS_Module?> GetByIdRentalModuleAsync(int moduleId);

        // Catalog (ONLY ONCE)
        Task<int> CreateCatalogModuleAsync(CT_Module request);
        Task<bool> UpdateCatalogModuleAsync(int id, CT_Module request);
        Task<IEnumerable<CT_Module>> GetAllCatalogModuleAsync();
        Task<CT_Module?> GetByIdCatalogModuleAsync(int id);
    }
}