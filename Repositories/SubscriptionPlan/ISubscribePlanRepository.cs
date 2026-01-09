using XeniaRegistrationBackend.Dtos;

namespace XeniaRegistrationBackend.Repositories.SubscriptionPlan
{
    public interface ISubscribePlanRepository
    {
        Task<int> CreateSubscribePlanAsync(SubscribePlanRequestDto request);
        Task<bool> CreateSubscribeUpdateAsync(int planId, SubscribePlanRequestDto request);
        Task<IEnumerable<SubscribePlanResponseDto>> GetAllSubscriptionPlanAsync();
        Task<SubscribePlanResponseDto?> GetSubscriptionPlanByIdAsync(int planId);



        Task<int> CreateSubscriptionAsync(CompanySubscriptionCreateDto dto);
        Task<int> CreateAddonAsync(CompanySubscriptionAddonCreateDto dto);
    }
}
