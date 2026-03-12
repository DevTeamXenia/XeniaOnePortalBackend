using XeniaRegistrationBackend.Models;
using XeniaRegistrationBackend.Models.Temple;

namespace XeniaRegistrationBackend.Repositories.Module
{
    public interface IModuleRepository
    {
        Task<int> CreateTempleModuleAsync(TK_Module request);
        Task<bool> UpdateTempleModuleAsync(int moduleId, TK_Module request);
        Task<IEnumerable<TK_Module>> GetAllTempleModuleAsync();
        Task<TK_Module?> GetByIdTempleModuleAsync(int moduleId);


        Task<int> CreateRentalModuleAsync(XRS_Module request);
        Task<bool> UpdateRentalModuleAsync(int moduleId, XRS_Module request);
        Task<IEnumerable<XRS_Module>> GetAllRentalModuleAsync();
        Task<XRS_Module?> GetByIdRentalModuleAsync(int moduleId);
    }

}
