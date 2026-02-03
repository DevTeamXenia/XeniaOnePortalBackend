using XeniaRegistrationBackend.Dtos;
using XeniaRegistrationBackend.Models.Token;

namespace XeniaRegistrationBackend.Repositories.SubscriptionPlan
{
    public interface ISubscribePlanRepository
    {

        #region TEMPLE
        Task<int> CreateTempleSubscribePlanAsync(SubscribeTemplePlanRequestDto request);
        Task<bool> CreateTempleSubscribeUpdateAsync(int planId, SubscribeTemplePlanRequestDto request);
        Task<IEnumerable<SubscribeTemplePlanResponseDto>> GetAllTempleSubscriptionPlanAsync();
        Task<SubscribeTemplePlanResponseDto?> GetSubscriptionTemplePlanByIdAsync(int planId);



        Task<int> CreateTempleSubscriptionAsync(CompanyTempleSubscriptionCreateDto dto);
        Task<int> CreateTempleAddonAsync(CompanyTempleSubscriptionAddonCreateDto dto);

        #endregion



        #region TOKEN
        Task<int> CreateTokenSubscribePlanAsync(SubscribeTokenPlanRequestDto request);
        Task<bool> CreateTokenSubscribeUpdateAsync(int planId, SubscribeTokenPlanRequestDto request);
        Task<IEnumerable<SubscribeTokenPlanResponseDto>> GetAllTokenSubscriptionPlanAsync();
        Task<SubscribeTokenPlanResponseDto?> GetSubscriptionTokenPlanByIdAsync(int planId);

        Task<int> CreateTokenSubscriptionAsync(CompanyTokenSubscriptionCreateDto dto);
        Task<int> CreateTokenAddonAsync(CompanyTokenSubscriptionAddonCreateDto dto);

        #endregion
    }
}
