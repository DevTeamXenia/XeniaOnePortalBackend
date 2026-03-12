using XeniaRegistrationBackend.Dtos;
using XeniaRegistrationBackend.Models;
using XeniaRegistrationBackend.Models.Temple;

namespace XeniaRegistrationBackend.Repositories.PlanModule
{
    public interface IPlanModuleMapRepository
    {
        Task<int> CreateTemplePlanModuleAsync(TK_PlanModuleMap request);
        Task<bool> UpdateTemplePlanModuleAsync(int subPlanId, TK_PlanModuleMap request);
        Task<PlanModuleMapResponseDto?> GetTemplePlanModuleByIdAsync(int subPlanId);
        Task<List<PlanModuleMapResponseDto>> GetTempleAllAsync();


        Task<int> CreateRentalPlanModuleAsync(XRS_PlanModuleMap request);
        Task<bool> UpdateRentalPlanModuleAsync(int subPlanId, XRS_PlanModuleMap request);
        Task<PlanModuleMapResponseDto?> GetRentalPlanModuleByIdAsync(int subPlanId);
        Task<List<PlanModuleMapResponseDto>> GetRentalAllAsync();
    }
}
