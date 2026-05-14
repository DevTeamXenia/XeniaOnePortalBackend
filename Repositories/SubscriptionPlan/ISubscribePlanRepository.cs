using XeniaRegistrationBackend.Dtos;
using XeniaRegistrationBackend.Models.Catalog;

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
        Task<int> CreateRentalAddonAsync(CompanyRentalSubscriptionAddonCreateDto dto);
        Task<SubscriptionRentalSummaryDto?> GetRentalSubscriptionSummaryAsync(int companyId);
       

        #endregion


        #region TICKET

        Task<int> CreateTicketSubscribePlanAsync(SubscribeTicketPlanRequestDto request);
        Task<bool> CreateTicketSubscribeUpdateAsync(int planId, SubscribeTicketPlanRequestDto request);
        Task<IEnumerable<SubscribeTicketPlanResponseDto>> GetAllTicketSubscriptionPlanAsync();
        Task<SubscribeTicketPlanResponseDto?> GetSubscriptionTicketPlanByIdAsync(int planId);

        Task<int> CreateTicketSubscriptionAsync(CompanyTicketSubscriptionCreateDto dto);

        #endregion
        #region CATALOG
        Task<int> CreateCatalogSubscribePlanAsync(SubscribeCataloguePlanRequestDto request);
        Task<bool> UpdateCatalogSubscribePlanAsync(int planId, SubscribeCataloguePlanRequestDto request);
        Task<bool> CreateCatalogSubscribeUpdateAsync(int planId, SubscribeCataloguePlanRequestDto request);
        Task<IEnumerable<SubscribeCatalogPlanResponseDto>> GetAllCatalogSubscriptionPlanAsync();
        Task<SubscribeCatalogPlanResponseDto?> GetSubscriptionCatalogPlanByIdAsync(int planId);
        Task<int> CreateCatalogSubscriptionAsync(CompanyCatalogSubscriptionCreateDto dto);
        Task<int> CreateCatalogAddonAsync(CompanyCatalogSubscriptionAddonCreateDto dto);
        Task<SubscriptionCatalogueSummaryDto?> GetCatalogSubscriptionSummaryAsync(int companyId);
        //Task<int> CreateCatalogSpecialRateAsync(CatalogSpecialRateCreateDto dto);
        //Task<bool> UpdateCatalogSpecialRateAsync(int id, CatalogSpecialRateCreateDto dto);
        //Task<IEnumerable<CatalogSpecialRateResponseDto>> GetCatalogSpecialRatesByCompanyAsync(int companyId);
        //Task<CatalogSpecialRateResponseDto?> GetCatalogSpecialRateByIdAsync(int id);
        #endregion
    }
}
