using Microsoft.EntityFrameworkCore;
using XeniaRegistrationBackend.Dtos;
using XeniaRegistrationBackend.Models;
using XeniaRegistrationBackend.Models.Rental;
using XeniaRegistrationBackend.Models.Temple;
using XeniaRegistrationBackend.Models.Ticket;
using XeniaRegistrationBackend.Models.Token;

namespace XeniaRegistrationBackend.Repositories.CompanyRegistration
{
    public class CompanyRegistrationRepository : ICompanyRegistrationRepository
    {
        private readonly TempleDbContext _tecontext;
        private readonly TokenDbContext _tocontext;
        private readonly RentalDbContext _recontext;
        private readonly TicketDbContext _ticontext;

        public CompanyRegistrationRepository(TempleDbContext tecontext, TokenDbContext tocontext, RentalDbContext recontext , TicketDbContext ticontext)
        {
            _tecontext = tecontext;
            _tocontext = tocontext;
            _recontext = recontext;
            _ticontext = ticontext;
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
            DateTime currentDate = DateTime.Now;

            var companies = await _tecontext.Company.ToListAsync();
            var result = new List<CompanyTempleListDto>();

            foreach (var c in companies)
            {
                var latestSub = await _tecontext.CompanySubscriptions
                    .Where(s => s.CompanyId == c.CompanyId && s.Status != "PENDING")
                    .OrderByDescending(s => s.SubscriptionEndDate)
                    .FirstOrDefaultAsync();

                SubscriptionTempleSummaryDto? subDto = null;

                if (latestSub != null)
                {
                    string realStatus = latestSub.Status.Trim().ToUpper();

                    if (realStatus == "ACTIVE" && latestSub.SubscriptionEndDate < currentDate)
                        realStatus = "EXPIRED";
                    else if (realStatus == "TRIAL" && latestSub.SubscriptionEndDate < currentDate)
                        realStatus = "TRIAL_EXPIRED";


                    var mainPlanName = await _tecontext.SubscribePlan
                        .Where(p => p.PlanId == latestSub.PlanId)
                        .Select(p => p.PlanName)
                        .FirstOrDefaultAsync();

                    var addons = await (
                        from a in _tecontext.CompanySubscriptionAddon
                        join p in _tecontext.SubscribePlan
                            on a.PlanId equals p.PlanId
                        where a.CompanyId == c.CompanyId
                              && a.MainPlanId == latestSub.PlanId
                              && a.Status == "ACTIVE"
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
                        Status = realStatus,
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
            DateTime currentDate = DateTime.Now;

            var company = await _tecontext.Company
                .FirstOrDefaultAsync(c => c.CompanyId == companyId);

            if (company == null)
                return null;

            var subscription = await _tecontext.CompanySubscriptions
                .Where(s => s.CompanyId == companyId && s.Status != "PENDING")
                .OrderByDescending(s => s.SubscriptionEndDate)
                .FirstOrDefaultAsync();

            SubscriptionTempleSummaryDto? subDto = null;

            if (subscription != null)
            {
                string realStatus = subscription.Status.Trim().ToUpper();

                if (realStatus == "ACTIVE" && subscription.SubscriptionEndDate < currentDate)
                    realStatus = "EXPIRED";
                else if (realStatus == "TRIAL" && subscription.SubscriptionEndDate < currentDate)
                    realStatus = "TRIAL_EXPIRED";

        
                var addons = await _tecontext.CompanySubscriptionAddon
                    .Where(a =>
                        a.CompanyId == companyId &&
                        a.MainPlanId == subscription.PlanId &&
                        a.Status == "ACTIVE")
                    .Select(a => new SubscriptionTempleAddonDto
                    {
                        SubAddonId = a.SubAddonId,
                        Amount = a.Amount,
                        UserCount = a.UserCount
                    })
                    .ToListAsync();

                subDto = new SubscriptionTempleSummaryDto
                {
                    SubId = subscription.SubId,
                    Status = realStatus,
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
                    })
                    .ToListAsync(),

                Labels = await _tecontext.CompanyLabel
                    .Where(l => l.CompanyId == companyId)
                    .Select(l => new CompanyLabelDto
                    {
                        SettingKey = l.SettingKey,
                        DisplayName = l.DisplayName,
                        DisplayNameTa = l.DisplayNameTa,
                        DisplayNameMa = l.DisplayNameMa
                    })
                    .ToListAsync()
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
         
                    var plan = await (
                        from p in _tecontext.SubscribePlan
                        where p.PlanId == latestSub.PlanId
                        select new
                        {
                            PlanName = p.PlanName                      
                        }
                    ).FirstOrDefaultAsync();


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
                        PlanName = plan?.PlanName ?? string.Empty,
                        PlanDuration = latestSub?.SubscriptionDays, 
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

            var subscription = await _tocontext.CompanySubscriptions
                .Where(s => s.CompanyId == companyId && s.Status != "PENDING")
                .OrderByDescending(s => s.SubscriptionEndDate)
                .FirstOrDefaultAsync();

            SubscriptionTokenSummaryDto? subDto = null;

            if (subscription != null)
            {

                var plan = await (
                    from p in _tecontext.SubscribePlan
                    where p.PlanId == subscription.PlanId
                    select new
                    {
                        PlanName = p.PlanName
                    }
                ).FirstOrDefaultAsync();

                subDto = new SubscriptionTokenSummaryDto
                {
                    SubId = subscription.SubId,
                    Status = subscription.Status,
                    StartDate = subscription.SubscriptionStartDate,
                    EndDate = subscription.SubscriptionEndDate,
                    Amount = subscription.SubscriptionAmount,
                    DepCount = subscription.SubscriptionDepCount,
                    PlanName = plan?.PlanName ?? string.Empty,
                    PlanDuration = subscription.SubscriptionDays 
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

            //await _tecontext.SaveChangesAsync();
            // ✅ FIX:
            await _tocontext.SaveChangesAsync();
        }





        public async Task<int> RegisterRentalCompanyAsync(CompanyRentalRegistrationRequestDto request)
        {
            //using var tx = await _tocontext.Database.BeginTransactionAsync();
            using var tx = await _recontext.Database.BeginTransactionAsync();  // ✅ FIX
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
                    Country = request.Country   // ✅ ADD THIS


                };

                _recontext.Company.Add(company);
                //await _tocontext.SaveChangesAsync();
                await _recontext.SaveChangesAsync();  // ✅ FIX

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
                    SubscriptionUserCount = 2,  // ✅ ADD THIS ONLY

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

        //public async Task<List<CompanyRentalListDto>> GetAllRentalCompaniesAsync()
        //{
        //    var result = await _recontext.Company
        //        .Select(c => new CompanyRentalListDto
        //        {
        //            CompanyId = c.companyID,
        //            CompanyName = c.companyName,
        //            Address = c.address,
        //            Email = c.email,
        //            PhoneNumber = c.phoneNumber,
        //            Pin = c.pin,
        //            Country = c.Country,// ✅ ADD THIS
        //            IsActive = c.IsActive,

        //            Subscription = _recontext.CompanySubscription
        //                .Where(s => s.CompanyId == c.companyID)
        //                .OrderByDescending(s => s.SubscriptionEndDate)
        //                .Select(s => new SubscriptionRentalSummaryDto
        //                {
        //                    SubId = s.SubId,
        //                    Status = s.Status,
        //                    StartDate = s.SubscriptionStartDate,
        //                    EndDate = s.SubscriptionEndDate,
        //                    Amount = s.SubscriptionAmount,

        //                    PlanName = _recontext.SubscribePlan
        //                        .Where(p => p.PlanId == s.PlanId)
        //                        .Select(p => p.PlanName)
        //                        .FirstOrDefault() ?? string.Empty,

        //                    DurationDays = _recontext.SubscribePlanDurations
        //                        .Where(d =>
        //                            d.PlanId == s.PlanId &&
        //                            d.Price == s.SubscriptionAmount &&
        //                            d.IsActive)
        //                        .OrderBy(d => d.DurationDays)
        //                        .Select(d => d.DurationDays)
        //                        .FirstOrDefault()
        //                })
        //                .FirstOrDefault()
        //        })
        //        .ToListAsync();

        //    return result;
        //}

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
                    var planName = await _recontext.SubscribePlan
                        .Where(p => p.PlanId == latestSub.PlanId)
                        .Select(p => p.PlanName)
                        .FirstOrDefaultAsync();

                    var durationDays = await _recontext.SubscribePlanDurations
                        .Where(d =>
                            d.PlanId == latestSub.PlanId &&
                            d.Price == latestSub.SubscriptionAmount &&
                            d.IsActive)
                        .OrderBy(d => d.DurationDays)
                        .Select(d => d.DurationDays)
                        .FirstOrDefaultAsync();

                    subDto = new SubscriptionRentalSummaryDto
                    {
                        SubId = latestSub.SubId,
                        Status = latestSub.Status,
                        StartDate = latestSub.SubscriptionStartDate,
                        EndDate = latestSub.SubscriptionEndDate,
                        Amount = latestSub.SubscriptionAmount,
                        UserCount = latestSub.SubscriptionUserCount,  // ✅ THIS IS YOUR TASK
                        PlanName = planName ?? string.Empty,
                        DurationDays = durationDays
                    };
                }

                result.Add(new CompanyRentalListDto
                {
                    CompanyId = c.companyID,
                    CompanyName = c.companyName,
                    Address = c.address,
                    Email = c.email,
                    PhoneNumber = c.phoneNumber,
                    Pin = c.pin,
                    Country = c.Country,
                    IsActive = c.IsActive,
                    Subscription = subDto
                });
            }

            return result;
        }



        public async Task<CompanyRentalDetailDto?> GetRentalCompanyByIdAsync(int companyId)
                     
        {
            var company = await _recontext.Company
                .Where(c => c.companyID == companyId)
                .Select(c => new CompanyRentalDetailDto
                {
                    Company = new CompanyRentalListDto
                    {
                        CompanyId = c.companyID,
                        CompanyName = c.companyName,
                        IsActive = c.IsActive,
                        PhoneNumber = c.phoneNumber,
                        Address = c.address,
                        Email = c.email,   // ✅ ADD THIS
                        Pin = c.pin,
                        Country = c.Country,



                        Subscription = _recontext.CompanySubscription
                            .Where(s => s.CompanyId == c.companyID)
                            .OrderByDescending(s => s.SubscriptionEndDate)
                            .Select(s => new SubscriptionRentalSummaryDto
                            {
                                SubId = s.SubId,
                                Status = s.Status,
                                StartDate = s.SubscriptionStartDate,
                                EndDate = s.SubscriptionEndDate,
                                Amount = s.SubscriptionAmount,

                                PlanName = _recontext.SubscribePlan
                                    .Where(p => p.PlanId == s.PlanId)
                                    .Select(p => p.PlanName)
                                    .FirstOrDefault() ?? string.Empty,

                                DurationDays = _recontext.SubscribePlanDurations
                                    .Where(d =>
                                        d.PlanId == s.PlanId &&
                                        d.Price == s.SubscriptionAmount &&
                                        d.IsActive)
                                    .OrderBy(d => d.DurationDays)
                                    .Select(d => d.DurationDays)
                                    .FirstOrDefault()
                            })
                            .FirstOrDefault()
                    },

                    Settings = _recontext.CompanySetting
                        .Where(s => s.CompanyId == c.companyID)
                        .Select(s => new CompanyRentalSettingsDto
                        {
                            KeyCode = s.KeyCode,
                            Value = s.Value
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            return company;
        }

        public async Task UpdateRentalCompanyAsync(UpdateRentalCompanyDto dto)
        {
            var company = await _recontext.Company.FirstOrDefaultAsync(c => c.companyID == dto.CompanyId);
            if (company == null) throw new Exception("Company not found");

            company.companyName = dto.CompanyName;
            company.address = dto.Address;
            company.Country = dto.Country;   // ✅ ADD THIS


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




        public async Task<int> RegisterTicketCompanyAsync(CompanyTicketRegistrationRequestDto request)
        {
            using var transaction = await _ticontext.Database.BeginTransactionAsync();

            try
            {
                var company = new TI_Company
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

                _ticontext.Company.Add(company);
                await _ticontext.SaveChangesAsync();


                var startDate = DateTime.Today;
                var endDate = startDate.AddDays(14);

                var trialSubscription = new TI_CompanySubscription
                {
                    CompanyId = company.CompanyId,
                    PlanId = 0,
                    SubscriptionDate = DateTime.Now,
                    SubscriptionStartDate = startDate,
                    SubscriptionEndDate = endDate,
                    SubscriptionDays = 14,
                    SubscriptionAmount = 0,
                    SubscriptionUserCount = 2,
                    Status = "TRIAL"
                };

                _ticontext.CompanySubscription.Add(trialSubscription);


                foreach (var label in request.Labels)
                {
                    _ticontext.CompanyLabel.Add(new TI_CompanyLabel
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
                    _ticontext.CompanySettings.Add(new TI_CompanySettings
                    {
                        CompanyId = company.CompanyId,
                        KeyCode = setting.KeyCode,
                        Value = setting.Value
                    });
                }

                await _ticontext.SaveChangesAsync();
                await transaction.CommitAsync();

                return company.CompanyId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<CompanyTicketListDto>> GetAllTicketCompaniesAsync()
        {
            var companies = await _ticontext.Company.ToListAsync();
            var result = new List<CompanyTicketListDto>();

            foreach (var c in companies)
            {
                var latestSub = await _ticontext.CompanySubscription
                    .Where(s => s.CompanyId == c.CompanyId)
                    .OrderByDescending(s => s.SubscriptionEndDate)
                    .FirstOrDefaultAsync();

                SubscriptionTicketSummaryDto? subDto = null;

                if (latestSub != null)
                {
                    var planName = await _ticontext.SubscribePlan
                        .Where(p => p.PlanId == latestSub.PlanId)
                        .Select(p => p.PlanName)
                        .FirstOrDefaultAsync();

                    subDto = new SubscriptionTicketSummaryDto
                    {
                        SubId = latestSub.SubId,
                        Status = latestSub.Status,
                        StartDate = latestSub.SubscriptionStartDate,
                        EndDate = latestSub.SubscriptionEndDate,
                        Amount = latestSub.SubscriptionAmount,
                        UserCount = latestSub.SubscriptionUserCount,
                        PlanName = planName ?? string.Empty,
                        PlanDuration = latestSub.SubscriptionDays 
                    };
                }

                result.Add(new CompanyTicketListDto
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

        public async Task<CompanyTicketDetailDto?> GetTicketCompanyByIdAsync(int companyId)
        {
            var company = await _ticontext.Company
                .FirstOrDefaultAsync(c => c.CompanyId == companyId);

            if (company == null)
                return null;

            var subscription = await _ticontext.CompanySubscription
                .Where(s => s.CompanyId == companyId)
                .OrderByDescending(s => s.SubscriptionEndDate)
                .FirstOrDefaultAsync();

            SubscriptionTicketSummaryDto? subDto = null;

            if (subscription != null)
            {
                var planName = await _ticontext.SubscribePlan
                    .Where(p => p.PlanId == subscription.PlanId)
                    .Select(p => p.PlanName)
                    .FirstOrDefaultAsync();

                subDto = new SubscriptionTicketSummaryDto
                {
                    SubId = subscription.SubId,
                    Status = subscription.Status,
                    StartDate = subscription.SubscriptionStartDate,
                    EndDate = subscription.SubscriptionEndDate,
                    Amount = subscription.SubscriptionAmount,
                    UserCount = subscription.SubscriptionUserCount,
                    PlanName = planName ?? string.Empty,
                    PlanDuration = subscription.SubscriptionDays
                };
            }

            return new CompanyTicketDetailDto
            {
                Company = new CompanyTicketListDto
                {
                    CompanyId = company.CompanyId,
                    CompanyName = company.CompanyName,
                    CompanyType = company.CompanyType,
                    PhoneNumber = company.CompanyPhone1,
                    Address = company.CompanyAddress,
                    Subscription = subDto
                },

                Settings = await _ticontext.CompanySettings
                    .Where(s => s.CompanyId == companyId)
                    .Select(s => new CompanyTicketSettingDto
                    {
                        KeyCode = s.KeyCode,
                        Value = s.Value
                    })
                    .ToListAsync(),

                Labels = await _ticontext.CompanyLabel
                    .Where(l => l.CompanyId == companyId)
                    .Select(l => new CompanyTicketLabelDto
                    {
                        SettingKey = l.SettingKey,
                        DisplayName = l.DisplayName,
                        DisplayNameTa = l.DisplayNameTa,
                        DisplayNameMa = l.DisplayNameMa
                    })
                    .ToListAsync()
            };
        }

        public async Task UpdateTicketCompanyAsync(UpdateTicketCompanyDto dto)
        {
            var company = await _ticontext.Company.FirstOrDefaultAsync(c => c.CompanyId == dto.CompanyId);
            if (company == null) throw new Exception("Company not found");

            company.CompanyName = dto.CompanyName;
            company.CompanyAddress = dto.Address;

            _ticontext.CompanySettings.RemoveRange(
                _ticontext.CompanySettings.Where(s => s.CompanyId == dto.CompanyId));

            _ticontext.CompanyLabel.RemoveRange(
                _ticontext.CompanyLabel.Where(l => l.CompanyId == dto.CompanyId));

            await _ticontext.SaveChangesAsync();

            _ticontext.CompanySettings.AddRange(dto.Settings.Select(s => new TI_CompanySettings
            {
                CompanyId = dto.CompanyId,
                KeyCode = s.KeyCode,
                Value = s.Value
            }));

            _ticontext.CompanyLabel.AddRange(dto.Labels.Select(l => new TI_CompanyLabel
            {
                CompanyId = dto.CompanyId,
                SettingKey = l.SettingKey,
                DisplayName = l.DisplayName,
                DisplayNameTa = l.DisplayNameTa,
                DisplayNameMa = l.DisplayNameMa
            }));

            await _tecontext.SaveChangesAsync();
        }


    }

}
