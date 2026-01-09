using XeniaRegistrationBackend.Dtos;
using XeniaTempleBackend.Models;

namespace XeniaRegistrationBackend.Repositories.PlanModule
{
    public interface IPlanModuleMapRepository
    {
        Task<int> CreatePlanModuleAsync(TK_PlanModuleMap request);
        Task<bool> UpdatePlanModuleAsync(int subPlanId, TK_PlanModuleMap request);
        Task<PlanModuleMapResponseDto?> GetPlanModuleByIdAsync(int subPlanId);
        Task<List<PlanModuleMapResponseDto>> GetAllAsync();
    }
}
