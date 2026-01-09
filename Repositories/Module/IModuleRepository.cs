using XeniaTempleBackend.Models;

namespace XeniaRegistrationBackend.Repositories.Module
{
    public interface IModuleRepository
    {
        Task<int> CreateModuleAsync(TK_Module request);
        Task<bool> UpdateModuleAsync(int moduleId, TK_Module request);
        Task<IEnumerable<TK_Module>> GetAllModuleAsync();
        Task<TK_Module?> GetByIdModuleAsync(int moduleId);
    }

}
