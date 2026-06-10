using XeniaRegistrationBackend.Dtos;
using XeniaRegistrationBackend.Models;
using XeniaRegistrationBackend.Models.Catalog;
using XeniaRegistrationBackend.Models.Temple;

namespace XeniaRegistrationBackend.Repositories.PlanModule
{
    public interface IPlanModuleMapRepository
    {
        Task<List<int>> CreateTemplePlanModuleAsync(List<TK_PlanModuleMap> request);
        Task<PlanModuleGroupResponseDto?> GetTemplePlanModuleByIdAsync(int subPlanId);
        Task<List<PlanModuleGroupResponseDto>> GetTempleAllAsync();


        Task<List<int>> CreateRentalPlanModuleAsync(List<XRS_PlanModuleMap> request);
        Task<PlanModuleGroupResponseDto?> GetRentalPlanModuleByIdAsync(int subPlanId);
        Task<List<PlanModuleGroupResponseDto>> GetRentalAllAsync();




        Task<List<int>> CreateCatalogPlanModuleAsync(List<CT_PlanModuleMap> request);
        Task<bool> UpdateCatalogPlanModuleAsync(List<CT_PlanModuleMap> request);
        Task<SubscriptionCatalogPlanResponseDto?> GetCatalogPlanModuleByIdAsync(int id);
        Task<List<SubscriptionCatalogPlanResponseDto>> GetCatalogAllAsync();
    }
}
