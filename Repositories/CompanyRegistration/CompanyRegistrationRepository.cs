using Microsoft.EntityFrameworkCore;
using XeniaRegistrationBackend.Dtos;
using XeniaRegistrationBackend.Models;
using XeniaRegistrationBackend.Models.Rental;
using XeniaRegistrationBackend.Models.Temple;
using XeniaRegistrationBackend.Models.Token;

namespace XeniaRegistrationBackend.Repositories.CompanyRegistration
{
    public class CompanyRegistrationRepository : ICompanyRegistrationRepository
    {
        private readonly TempleDbContext _tecontext;
        private readonly TokenDbContext _tocontext;
        private readonly RentalDbContext _recontext;

        public CompanyRegistrationRepository(TempleDbContext tecontext, TokenDbContext tocontext, RentalDbContext recontext)
        {
            _tecontext = tecontext;
            _tocontext = tocontext;
            _recontext = recontext;
        }

        public async Task<int> RegisterTempleCompanyAsync( CompanyTempleRegistrationRequestDto request)
        {
            using var transaction = await _tecontext.Database.BeginTransactionAsync();

            try
            {
                var company = new TK_Company
                {
                    CompanyName = request.CompanyName,
                    CompanyAddress = request.CompanyAddress,
                    CompanyPhone1 = request.Phone1,
                    CompanyPhone2 = request.Phone2,
                    CompanyRegNo = request.RegNo,
                    CompanyType = request.CompanyType,
                    DistrictName = request.District,
                    StateName = request.State,
                    IFSCCode = request.IFSCCode,
                    CompanyToken = Guid.NewGuid().ToString(),
                    CompanyCreatedBy = 0
                };

                _tecontext.Company.Add(company);
                await _tecontext.SaveChangesAsync();

           
                var startDate = DateTime.Today;
                var endDate = startDate.AddDays(14);

                var trialSubscription = new TK_CompanySubscription
                {
                    CompanyId = company.CompanyId,
                    PlanId = 0,                
                    SubscriptionDate = DateTime.Now,
                    SubscriptionStartDate = startDate,
                    SubscriptionEndDate = endDate,
                    SubscriptionDays = 14,
                    SubscriptionAmount = 0,
                    subscriptionUserCount = 2,
                    Status = "TRIAL"
                };

                _tecontext.CompanySubscriptions.Add(trialSubscription);


                foreach (var label in request.Labels)
                {
                    _tecontext.CompanyLabel.Add(new TK_CompanyLabel
                    {
                        CompanyId = company.CompanyId,
                        SettingKey = label.SettingKey,
                        DisplayName = label.DisplayName,
                        DisplayNameTa = label.DisplayNameTa,
                        DisplayNameMa = label.DisplayNameMa,
                        CreatedBy = 0
                    });
                }

              
                foreach (var setting in request.Settings)
                {
                    _tecontext.CompanySetting.Add(new TK_CompanySettings
                    {
                        CompanyId = company.CompanyId,
                        KeyCode = setting.KeyCode,
                        Value = setting.Value
                    });
                }

                await _tecontext.SaveChangesAsync();
                await transaction.CommitAsync();

                return company.CompanyId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<CompanyTempleListDto>> GetAllTempleCompaniesAsync()
        {
            var companies = await _tecontext.Company.ToListAsync();
            var result = new List<CompanyTempleListDto>();

            foreach (var c in companies)
            {
                var latestSub = await _tecontext.CompanySubscriptions
                    .Where(s => s.CompanyId == c.CompanyId)
                    .OrderByDescending(s => s.SubscriptionEndDate)
                    .FirstOrDefaultAsync();

                SubscriptionTempleSummaryDto? subDto = null;

                if (latestSub != null)
                {

                    var mainPlanName = await _tecontext.SubscribePlan
                        .Where(p => p.PlanId == latestSub.PlanId)
                        .Select(p => p.PlanName)
                        .FirstOrDefaultAsync();


                    var addons = await (
                        from a in _tecontext.CompanySubscriptionAddon
                        join p in _tecontext.SubscribePlan
                            on a.PlanId equals p.PlanId
                        where a.CompanyId == c.CompanyId
                              && a.MainPlanId == latestSub.SubId
                        select new SubscriptionTempleAddonDto
                        {
                            SubAddonId = a.SubAddonId,
                            SubAddonName = p.PlanName,
                            Amount = a.Amount,
                            UserCount = a.UserCount
                        }
                    ).ToListAsync();

                    subDto = new SubscriptionTempleSummaryDto
                    {
                        SubId = latestSub.SubId,
                        Status = latestSub.Status,
                        StartDate = latestSub.SubscriptionStartDate,
                        EndDate = latestSub.SubscriptionEndDate,
                        Amount = latestSub.SubscriptionAmount,
                        UserCount = latestSub.subscriptionUserCount,
                        PlanName = mainPlanName ?? string.Empty,
                        Addons = addons.Any() ? addons : null
                    };
                }

                result.Add(new CompanyTempleListDto
                {
                    CompanyId = c.CompanyId,
                    CompanyName = c.CompanyName,
                    CompanyType = c.CompanyType,
                    PhoneNumber = c.CompanyPhone1,
                    Address = c.CompanyAddress,
                    Subscription = subDto
                });
            }

            return result;
        }

        public async Task<CompanyTempleDetailDto?> GetTempleCompanyByIdAsync(int companyId)
        {
            var company = await _tecontext.Company.FirstOrDefaultAsync(c => c.CompanyId == companyId);
            if (company == null) return null;

            var subscription = await _tecontext.CompanySubscriptions
                .Where(s => s.CompanyId == companyId)
                .OrderByDescending(s => s.SubscriptionEndDate)
                .FirstOrDefaultAsync();

            SubscriptionTempleSummaryDto? subDto = null;

            if (subscription != null)
            {
                var addons = await _tecontext.CompanySubscriptionAddon
                    .Where(a => a.CompanyId == companyId && a.MainPlanId == subscription.PlanId)
                    .Select(a => new SubscriptionTempleAddonDto
                    {
                        SubAddonId = a.SubAddonId,
                        Amount = a.Amount,
                        UserCount = a.UserCount
                    }).ToListAsync();

                subDto = new SubscriptionTempleSummaryDto
                {
                    SubId = subscription.SubId,
                    Status = subscription.Status,
                    StartDate = subscription.SubscriptionStartDate,
                    EndDate = subscription.SubscriptionEndDate,
                    Amount = subscription.SubscriptionAmount,
                    UserCount = subscription.subscriptionUserCount,
                    Addons = addons.Any() ? addons : null
                };
            }

            return new CompanyTempleDetailDto
            {
                Company = new CompanyTempleListDto
                {
                    CompanyId = company.CompanyId,
                    CompanyName = company.CompanyName,
                    CompanyType = company.CompanyType,
                    PhoneNumber = company.CompanyPhone1,
                    Address = company.CompanyAddress,
                    Subscription = subDto
                },
                Settings = await _tecontext.CompanySetting
                    .Where(s => s.CompanyId == companyId)
                    .Select(s => new CompanySettingDto
                    {
                        KeyCode = s.KeyCode,
                        Value = s.Value
                    }).ToListAsync(),

                Labels = await _tecontext.CompanyLabel
                    .Where(l => l.CompanyId == companyId)
                    .Select(l => new CompanyLabelDto
                    {
                        SettingKey = l.SettingKey,
                        DisplayName = l.DisplayName,
                        DisplayNameTa = l.DisplayNameTa,
                        DisplayNameMa = l.DisplayNameMa
                    }).ToListAsync()
            };
        }

        public async Task UpdateTempleCompanyAsync(UpdateTempleCompanyDto dto)
        {
            var company = await _tecontext.Company.FirstOrDefaultAsync(c => c.CompanyId == dto.CompanyId);
            if (company == null) throw new Exception("Company not found");

            company.CompanyName = dto.CompanyName;
            company.CompanyAddress = dto.Address;

            _tecontext.CompanySetting.RemoveRange(
                _tecontext.CompanySetting.Where(s => s.CompanyId == dto.CompanyId));

            _tecontext.CompanyLabel.RemoveRange(
                _tecontext.CompanyLabel.Where(l => l.CompanyId == dto.CompanyId));

            await _tecontext.SaveChangesAsync();

            _tecontext.CompanySetting.AddRange(dto.Settings.Select(s => new TK_CompanySettings
            {
                CompanyId = dto.CompanyId,
                KeyCode = s.KeyCode,
                Value = s.Value
            }));

            _tecontext.CompanyLabel.AddRange(dto.Labels.Select(l => new TK_CompanyLabel
            {
                CompanyId = dto.CompanyId,
                SettingKey = l.SettingKey,
                DisplayName = l.DisplayName,
                DisplayNameTa = l.DisplayNameTa,
                DisplayNameMa = l.DisplayNameMa
            }));

            await _tecontext.SaveChangesAsync();
        }



        public async Task<int> RegisterTokenCompanyAsync(CompanyTokenRegistrationRequestDto request)
        {
            using var tx = await _tocontext.Database.BeginTransactionAsync();

            try
            {
                var company = new xtm_Company
                {
                    CompanyName = request.CompanyName,
                    Status = request.Status,
                    Country = request.Country,
                    Address = request.Address,
                    Email = request.Email
                };

                _tocontext.Company.Add(company);
                await _tocontext.SaveChangesAsync();

                var startDate = DateTime.Today;
                var endDate = startDate.AddDays(14);

                var trialSubscription = new xtm_CompanySubscription
                {
                    CompanyId = company.CompanyID,
                    PlanId = 0, 
                    SubscriptionDate = DateTime.UtcNow,
                    SubscriptionStartDate = startDate,
                    SubscriptionEndDate = endDate,
                    SubscriptionDays = 14,
                    SubscriptionAmount = 0,
                    SubscriptionDepCount = 2,
                    Status = "TRIAL",
                    CreatedAt = DateTime.UtcNow
                };

                _tocontext.CompanySubscriptions.Add(trialSubscription);

          
                var companySettings = new xtm_CompanySettings
                {
                    CompanyId = company.CompanyID,
                    CollectCustomerName = request.Settings.CollectCustomerName,
                    PrintCustomerName = request.Settings.PrintCustomerName,
                    CollectCustomerMobileNumber = request.Settings.CollectCustomerMobileNumber,
                    PrintCustomerMobileNumber = request.Settings.PrintCustomerMobileNumber,
                    IsCustomCall = request.Settings.IsCustomCall,
                    IsServiceEnable = request.Settings.IsServiceEnable
                };

                _tocontext.CompanySettings.Add(companySettings);

                await _tocontext.SaveChangesAsync();
                await tx.CommitAsync();

                return company.CompanyID;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<List<CompanyTokenListDto>> GetAllTokenCompaniesAsync()
        {
            var companies = await _tocontext.Company.ToListAsync();
            var result = new List<CompanyTokenListDto>();

            foreach (var c in companies)
            {
                var latestSub = await _tocontext.CompanySubscriptions
                    .Where(s => s.CompanyId == c.CompanyID)
                    .OrderByDescending(s => s.SubscriptionEndDate)
                    .FirstOrDefaultAsync();

                SubscriptionTokenSummaryDto? subDto = null;

                if (latestSub != null)
                {

                    var mainPlanName = await _tecontext.SubscribePlan
                        .Where(p => p.PlanId == latestSub.PlanId)
                        .Select(p => p.PlanName)
                        .FirstOrDefaultAsync();


                    var addons = await (
                        from a in _tecontext.CompanySubscriptionAddon
                        join p in _tecontext.SubscribePlan
                            on a.PlanId equals p.PlanId
                        where a.CompanyId == c.CompanyID
                              && a.MainPlanId == latestSub.SubId
                        select new SubscriptionTokenAddonDto
                        {
                            SubAddonId = a.SubAddonId,
                            SubAddonName = p.PlanName,
                            Amount = a.Amount,
                            DepCount = a.UserCount
                        }
                    ).ToListAsync();

                    subDto = new SubscriptionTokenSummaryDto
                    {
                        SubId = latestSub.SubId,
                        Status = latestSub.Status,
                        StartDate = latestSub.SubscriptionStartDate,
                        EndDate = latestSub.SubscriptionEndDate,
                        Amount = latestSub.SubscriptionAmount,
                        DepCount = latestSub.SubscriptionDepCount,
                        PlanName = mainPlanName ?? string.Empty,
                        Addons = addons.Any() ? addons : null
                    };
                }

                result.Add(new CompanyTokenListDto
                {
                    CompanyId = c.CompanyID,
                    CompanyName = c.CompanyName,
                    Status = c.Status,
                    Country = c.Country,
                    Address = c.Address,
                    Email = c.Email,
                    Subscription = subDto
                });
            }

            return result;
        }

        public async Task<CompanyTokenDetailDto?> GetTokenCompanyByIdAsync(int companyId)
        {
            var company = await _tocontext.Company
                .FirstOrDefaultAsync(c => c.CompanyID == companyId);

            if (company == null)
                return null;

   
            var subscription = await _tecontext.CompanySubscriptions
                .Where(s => s.CompanyId == companyId && s.Status != "PENDING")
                .OrderByDescending(s => s.SubscriptionEndDate)
                .FirstOrDefaultAsync();

            SubscriptionTokenSummaryDto? subDto = null;

            if (subscription != null)
            {
                subDto = new SubscriptionTokenSummaryDto
                {
                    SubId = subscription.SubId,
                    Status = subscription.Status,
                    StartDate = subscription.SubscriptionStartDate,
                    EndDate = subscription.SubscriptionEndDate,
                    Amount = subscription.SubscriptionAmount,
                    DepCount = subscription.subscriptionUserCount
                };
            }

      
            var settingEntity = await _tocontext.CompanySettings
                .FirstOrDefaultAsync(s => s.CompanyId == companyId);

            CompanyTokenSettingsDto settingsDto = new CompanyTokenSettingsDto();

            if (settingEntity != null)
            {
                settingsDto = new CompanyTokenSettingsDto
                {
                    CollectCustomerName = settingEntity.CollectCustomerName,
                    PrintCustomerName = settingEntity.PrintCustomerName,
                    CollectCustomerMobileNumber = settingEntity.CollectCustomerMobileNumber,
                    PrintCustomerMobileNumber = settingEntity.PrintCustomerMobileNumber,
                    IsCustomCall = settingEntity.IsCustomCall,
                    IsServiceEnable = settingEntity.IsServiceEnable
                };
            }

            return new CompanyTokenDetailDto
            {
                Company = new CompanyTokenListDto
                {
                    CompanyId = company.CompanyID,
                    CompanyName = company.CompanyName,
                    Status = company.Status,
                    Country = company.Country,
                    Address = company.Address,
                    Email = company.Email,
                    Subscription = subDto
                },
                Settings = settingsDto
            };
        }

        public async Task UpdateTokenCompanyAsync(UpdateTokenCompanyDto dto)
        {
            var company = await _tocontext.Company
                .FirstOrDefaultAsync(c => c.CompanyID == dto.CompanyID);

            if (company == null)
                throw new Exception("Company not found");

       
            company.CompanyName = dto.CompanyName;
            company.Address = dto.Address;
            company.Status = dto.Status;
            company.Email = dto.Email;
            company.Country = dto.Country;

            var settings = await _tocontext.CompanySettings
                .FirstOrDefaultAsync(s => s.CompanyId == dto.CompanyID);

            var dtoSettings = dto.Settings.FirstOrDefault();

            if (dtoSettings != null)
            {
                if (settings == null)
                {
                    settings = new xtm_CompanySettings
                    {
                        CompanyId = dto.CompanyID,
                        CollectCustomerName = dtoSettings.CollectCustomerName,
                        PrintCustomerName = dtoSettings.PrintCustomerName,
                        CollectCustomerMobileNumber = dtoSettings.CollectCustomerMobileNumber,
                        PrintCustomerMobileNumber = dtoSettings.PrintCustomerMobileNumber,
                        IsCustomCall = dtoSettings.IsCustomCall,
                        IsServiceEnable = dtoSettings.IsServiceEnable,

                    };

                    await _tocontext.CompanySettings.AddAsync(settings);
                }
                else
                {        
                    settings.CollectCustomerName = dtoSettings.CollectCustomerName;
                    settings.PrintCustomerName = dtoSettings.PrintCustomerName;
                    settings.CollectCustomerMobileNumber = dtoSettings.CollectCustomerMobileNumber;
                    settings.PrintCustomerMobileNumber = dtoSettings.PrintCustomerMobileNumber;
                    settings.IsCustomCall = dtoSettings.IsCustomCall;
                    settings.IsServiceEnable = dtoSettings.IsServiceEnable;
                }
            }

            await _tecontext.SaveChangesAsync();
        }





        public async Task<int> RegisterRentalCompanyAsync(CompanyRentalRegistrationRequestDto request)
        {
            using var tx = await _tocontext.Database.BeginTransactionAsync();

            try
            {
                var company = new XRS_Company
                {
                    companyName = request.companyName,
                    address = request.address,
                    email = request.email,
                    phoneNumber = request.phoneNumber,
                    pin = request.pin,
                    logo = request.logo,
                    IsActive = request.IsActive,
                };

                _recontext.Company.Add(company);
                await _tocontext.SaveChangesAsync();

                var startDate = DateTime.Today;
                var endDate = startDate.AddDays(14);

                var trialSubscription = new XRS_CompanySubscription
                {
                    CompanyId = company.companyID,
                    PlanId = 0,
                    SubscriptionDate = DateTime.Now,
                    SubscriptionStartDate = startDate,
                    SubscriptionEndDate = endDate,
                    SubscriptionDays = 14,
                    SubscriptionAmount = 0,    
                    Status = "TRIAL",
                };

                _recontext.CompanySubscription.Add(trialSubscription);


                foreach (var setting in request.Settings)
                {
                    _recontext.CompanySetting.Add(new XRS_CompanySettings
                    {
                        CompanyId = company.companyID,
                        KeyCode = setting.KeyCode,
                        Value = setting.Value
                    });
                }
    
                await _recontext.SaveChangesAsync();
                await tx.CommitAsync();

                return company.companyID;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<List<CompanyRentalListDto>> GetAllRentalCompaniesAsync()
        {
            var companies = await _recontext.Company.ToListAsync();
            var result = new List<CompanyRentalListDto>();

            foreach (var c in companies)
            {
                var latestSub = await _recontext.CompanySubscription
                    .Where(s => s.CompanyId == c.companyID)
                    .OrderByDescending(s => s.SubscriptionEndDate)
                    .FirstOrDefaultAsync();

                SubscriptionRentalSummaryDto? subDto = null;

                if (latestSub != null)
                {

                    var mainPlanName = await _recontext.SubscribePlan
                        .Where(p => p.PlanId == latestSub.PlanId)
                        .Select(p => p.PlanName)
                        .FirstOrDefaultAsync();


                    subDto = new SubscriptionRentalSummaryDto
                    {
                        SubId = latestSub.SubId,
                        Status = latestSub.Status,
                        StartDate = latestSub.SubscriptionStartDate,
                        EndDate = latestSub.SubscriptionEndDate,
                        Amount = latestSub.SubscriptionAmount,                    
                        PlanName = mainPlanName ?? string.Empty,
                    };
                }

                result.Add(new CompanyRentalListDto
                {
                    CompanyName = c.companyName,
                    Address = c.address,
                    Email = c.email,
                    PhoneNumber = c.phoneNumber,
                    IsActive = c.IsActive,
                    Subscription = subDto
                });
            }

            return result;
        }

        public async Task<CompanyRentalDetailDto?> GetRentalCompanyByIdAsync(int companyId)
        {
            var company = await _recontext.Company.FirstOrDefaultAsync(c => c.companyID == companyId);
            if (company == null) return null;

            var subscription = await _recontext.CompanySubscription
                .Where(s => s.CompanyId == companyId)
                .OrderByDescending(s => s.SubscriptionEndDate)
                .FirstOrDefaultAsync();

            SubscriptionRentalSummaryDto? subDto = null;

            if (subscription != null)
            {
               subDto = new SubscriptionRentalSummaryDto
               {
                    SubId = subscription.SubId,
                    Status = subscription.Status,
                    StartDate = subscription.SubscriptionStartDate,
                    EndDate = subscription.SubscriptionEndDate,
                    Amount = subscription.SubscriptionAmount,
                };
            }

            return new CompanyRentalDetailDto
            {
                Company = new CompanyRentalListDto
                {
                    CompanyId = company.companyID,
                    CompanyName = company.companyName,
                    IsActive = company.IsActive,
                    PhoneNumber = company.phoneNumber,
                    Address = company.address,
                    Subscription = subDto
                },
                Settings = await _recontext.CompanySetting
                    .Where(s => s.CompanyId == companyId)
                    .Select(s => new CompanyRentalSettingsDto
                    {
                        KeyCode = s.KeyCode,
                        Value = s.Value
                    }).ToListAsync(),
            };
        }

        public async Task UpdateRentalCompanyAsync(UpdateRentalCompanyDto dto)
        {
            var company = await _recontext.Company.FirstOrDefaultAsync(c => c.companyID == dto.CompanyId);
            if (company == null) throw new Exception("Company not found");

            company.companyName = dto.CompanyName;
            company.address = dto.Address;

            _recontext.CompanySetting.RemoveRange(
                _recontext.CompanySetting.Where(s => s.CompanyId == dto.CompanyId));

            await _recontext.SaveChangesAsync();

            _recontext.CompanySetting.AddRange(dto.Settings.Select(s => new XRS_CompanySettings
            {
                CompanyId = dto.CompanyId,
                KeyCode = s.KeyCode,
                Value = s.Value
            }));

        

            await _recontext.SaveChangesAsync();
        }

    }

}
