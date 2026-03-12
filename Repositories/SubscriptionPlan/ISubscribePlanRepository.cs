using XeniaRegistrationBackend.Dtos;

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
        Task<List<SubscribeTokenPlanResponseDto>> GetAllTokenSubscriptionPlanAsync();
        Task<SubscribeTokenPlanResponseDto?> GetSubscriptionTokenPlanByIdAsync(int planId);


        Task<int> CreateTokenSubscriptionAsync(CompanyTokenSubscriptionCreateDto dto);
        Task<int> CreateTokenAddonAsync(CompanyTokenSubscriptionAddonCreateDto dto);

        #endregion


        #region RENTAL

        Task<int> CreateRentalSubscribePlanAsync(SubscribeRentalPlanRequestDto request);
        Task<bool> CreateRentalSubscribeUpdateAsync(int planId, SubscribeRentalPlanRequestDto request);
        Task<IEnumerable<SubscribeRentalPlanResponseDto>> GetAllRentalSubscriptionPlanAsync();
        Task<SubscribeRentalPlanResponseDto?> GetSubscriptionRentalPlanByIdAsync(int planId);


        Task<int> CreateRentalSubscriptionAsync(CompanyRentalSubscriptionCreateDto dto);

        #endregion


        #region TICKET

        Task<int> CreateTicketSubscribePlanAsync(SubscribeTicketPlanRequestDto request);
        Task<bool> CreateTicketSubscribeUpdateAsync(int planId, SubscribeTicketPlanRequestDto request);
        Task<IEnumerable<SubscribeTicketPlanResponseDto>> GetAllTicketSubscriptionPlanAsync();
        Task<SubscribeTicketPlanResponseDto?> GetSubscriptionTicketPlanByIdAsync(int planId);

        Task<int> CreateTicketSubscriptionAsync(CompanyTicketSubscriptionCreateDto dto);

        #endregion


    }
}
