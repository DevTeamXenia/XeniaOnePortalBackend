using XeniaRegistrationBackend.Dtos;
using XeniaRegistrationBackend.Models.Token;

namespace XeniaRegistrationBackend.Repositories.SubscriptionPlan
{
    public interface ISubscribePlanRepository
    {

        #region TEMPLE
        Task<int> CreateTempleSubscribePlanAsync(SubscribePlanRequestDto request);
        Task<bool> CreateTempleSubscribeUpdateAsync(int planId, SubscribePlanRequestDto request);
        Task<IEnumerable<SubscribePlanResponseDto>> GetAllTempleSubscriptionPlanAsync();
        Task<SubscribePlanResponseDto?> GetSubscriptionTemplePlanByIdAsync(int planId);



        Task<int> CreateTempleSubscriptionAsync(CompanySubscriptionCreateDto dto);
        Task<int> CreateTempleAddonAsync(CompanySubscriptionAddonCreateDto dto);

        #endregion



        #region TOKEN
        Task<IEnumerable<xtm_SubscribePlan>> GetAllTokenSubscriptionPlanAsync();
        Task<xtm_SubscribePlan?> GetSubscriptionTokenPlanByIdAsync(int planId);
        Task<xtm_SubscribePlan> CreateTokenSubscribePlanAsync(xtm_SubscribePlan plan);
        Task CreateTokenSubscribeUpdateAsync(xtm_SubscribePlan plan);

        #endregion
    }
}
