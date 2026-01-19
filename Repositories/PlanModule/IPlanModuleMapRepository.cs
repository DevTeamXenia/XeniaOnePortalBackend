using XeniaRegistrationBackend.Dtos;
using XeniaRegistrationBackend.Models.Temple;

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
