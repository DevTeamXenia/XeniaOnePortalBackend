using XeniaRegistrationBackend.Dtos;
using XeniaRegistrationBackend.Models;
using XeniaRegistrationBackend.Models.Catalog;
using XeniaRegistrationBackend.Models.Temple;

namespace XeniaRegistrationBackend.Repositories.PlanModule
{
    public interface IPlanModuleMapRepository
    {
        Task<List<int>> CreateTemplePlanModuleAsync(List<TK_PlanModuleMap> request);
        Task<bool> UpdateTemplePlanModuleAsync(List<TK_PlanModuleMap> request);
        Task<PlanModuleMapResponseDto?> GetTemplePlanModuleByIdAsync(int subPlanId);
        Task<List<PlanModuleMapResponseDto>> GetTempleAllAsync();


        Task<int> CreateRentalPlanModuleAsync(XRS_PlanModuleMap request);
        Task<bool> UpdateRentalPlanModuleAsync(int subPlanId, XRS_PlanModuleMap request);
        Task<PlanModuleMapResponseDto?> GetRentalPlanModuleByIdAsync(int subPlanId);
        Task<List<PlanModuleMapResponseDto>> GetRentalAllAsync();




        Task<List<int>> CreateCatalogPlanModuleAsync(List<CT_PlanModuleMap> request);
        Task<bool> UpdateCatalogPlanModuleAsync(List<CT_PlanModuleMap> request);
        Task<SubscriptionCatalogPlanResponseDto?> GetCatalogPlanModuleByIdAsync(int id);
        Task<List<SubscriptionCatalogPlanResponseDto>> GetCatalogAllAsync();
    }
}
