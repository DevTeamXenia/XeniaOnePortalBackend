

using Microsoft.EntityFrameworkCore;
using XeniaRegistrationBackend.Dtos;
using XeniaRegistrationBackend.Models;
using XeniaRegistrationBackend.Models.Rental;
using XeniaRegistrationBackend.Models.Temple;
using XeniaRegistrationBackend.Models.Ticket;
using XeniaRegistrationBackend.Models.Token;
using XeniaRegistrationBackend.Models.Catalog;
using YourProjectNamespace.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace XeniaRegistrationBackend.Repositories.CompanyRegistration
{
    public class CompanyRegistrationRepository : ICompanyRegistrationRepository
    {
        private readonly TempleDbContext _tecontext;
        private readonly TokenDbContext _tocontext;
        private readonly RentalDbContext _recontext;
        private readonly TicketDbContext _ticontext;
        private readonly CatalogDbContext _cacontext;

        public CompanyRegistrationRepository(TempleDbContext tecontext, TokenDbContext tocontext, RentalDbContext recontext, TicketDbContext ticontext, CatalogDbContext cacontext)
        {
            _tecontext = tecontext;
            _tocontext = tocontext;
            _recontext = recontext;
            _ticontext = ticontext;
            _cacontext = cacontext;
        }

        #region TEMPLE

        public async Task<int> RegisterTempleCompanyAsync(CompanyTempleRegistrationRequestDto request)
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
                    CompanyCreatedBy = 0,
                };

                _tecontext.Company.Add(company);
                await _tecontext.SaveChangesAsync();

                if (!string.IsNullOrEmpty(request.UserName) && !string.IsNullOrEmpty(request.Password))
                {
                    _tecontext.Users.Add(new TK_Users
                    {
                        CompanyId = company.CompanyId,
                        UserName = request.UserName,
                        Password = request.Password,
                        UserType = "ADMIN",
                        UserCreatedOn = DateTime.Now,
                        UserCreatedBy = 0,
                        UserStatus = true
                    });
                }

                var startDate = DateTime.Today;
                var endDate = startDate.AddDays(14);

                _tecontext.CompanySubscriptions.Add(new TK_CompanySubscription
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
                });

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
                        join p in _tecontext.SubscribePlan on a.PlanId equals p.PlanId
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
                    Phone2 = c.CompanyPhone2,
                    RegNo = c.CompanyRegNo,
                    District = c.DistrictName,
                    State = c.StateName,
                    IFSCCode = c.IFSCCode,
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
            if (company == null) return null;

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
                    .Where(a => a.CompanyId == companyId && a.MainPlanId == subscription.PlanId && a.Status == "ACTIVE")
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
                    Phone2 = company.CompanyPhone2,
                    RegNo = company.CompanyRegNo,
                    District = company.DistrictName,
                    State = company.StateName,
                    IFSCCode = company.IFSCCode,
                    Address = company.CompanyAddress,
                    Subscription = subDto
                },
                Settings = await _tecontext.CompanySetting
                    .Where(s => s.CompanyId == companyId)
                    .Select(s => new CompanySettingDto { KeyCode = s.KeyCode, Value = s.Value })
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
            using var transaction = await _tecontext.Database.BeginTransactionAsync();
            try
            {
                var company = await _tecontext.Company.FirstOrDefaultAsync(c => c.CompanyId == dto.CompanyId);
                if (company == null) throw new Exception("Company not found");

                company.CompanyName = dto.CompanyName;
                company.CompanyAddress = dto.Address;
                company.CompanyPhone1 = dto.Phone1;
                company.CompanyPhone2 = dto.Phone2;
                company.CompanyRegNo = dto.RegNo;
                company.CompanyType = dto.CompanyType;
                company.DistrictName = dto.District;
                company.StateName = dto.State;
                company.IFSCCode = dto.IFSCCode;

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
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        #endregion

        #region TOKEN

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

                var user = new XtmUser
                {
                    CompanyID = company.CompanyID,
                    Username = request.UserName,
                    Password = request.Password,
                    UserType = "Admin",
                    Status = true,
                    TokenResetAllowed = true
                };

                _tocontext.Users.Add(user);

                var startDate = DateTime.Today;
                var endDate = startDate.AddDays(14);

                _tocontext.CompanySubscriptions.Add(new xtm_CompanySubscription
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
                });

                _tocontext.CompanySettings.Add(new xtm_CompanySettings
                {
                    CompanyId = company.CompanyID,
                    CollectCustomerName = request.Settings.CollectCustomerName,
                    PrintCustomerName = request.Settings.PrintCustomerName,
                    CollectCustomerMobileNumber = request.Settings.CollectCustomerMobileNumber,
                    PrintCustomerMobileNumber = request.Settings.PrintCustomerMobileNumber,
                    IsCustomCall = request.Settings.IsCustomCall,
                    IsServiceEnable = request.Settings.IsServiceEnable
                });

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
                        select new { p.PlanName }
                    ).FirstOrDefaultAsync();

                    var addons = await (
                        from a in _tecontext.CompanySubscriptionAddon
                        join p in _tecontext.SubscribePlan on a.PlanId equals p.PlanId
                        where a.CompanyId == c.CompanyID && a.MainPlanId == latestSub.SubId
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
                        PlanDuration = latestSub.SubscriptionDays,
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
            if (company == null) return null;

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
                    select new { p.PlanName }
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

            var settingsDto = new CompanyTokenSettingsDto();
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
            if (company == null) throw new Exception("Company not found");

            company.CompanyName = dto.CompanyName;
            company.Address = dto.Address;
            company.Status = dto.Status;
            company.Email = dto.Email;
            company.Country = dto.Country;

            var settings = await _tocontext.CompanySettings.FirstOrDefaultAsync(s => s.CompanyId == dto.CompanyID);
            var dtoSettings = dto.Settings.FirstOrDefault();

            if (dtoSettings != null)
            {
                if (settings == null)
                {
                    await _tocontext.CompanySettings.AddAsync(new xtm_CompanySettings
                    {
                        CompanyId = dto.CompanyID,
                        CollectCustomerName = dtoSettings.CollectCustomerName,
                        PrintCustomerName = dtoSettings.PrintCustomerName,
                        CollectCustomerMobileNumber = dtoSettings.CollectCustomerMobileNumber,
                        PrintCustomerMobileNumber = dtoSettings.PrintCustomerMobileNumber,
                        IsCustomCall = dtoSettings.IsCustomCall,
                        IsServiceEnable = dtoSettings.IsServiceEnable,
                    });
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

            await _tocontext.SaveChangesAsync();
        }

        #endregion

        #region RENTAL

        public async Task<int> RegisterRentalCompanyAsync(CompanyRentalRegistrationRequestDto request)
        {
            using var tx = await _recontext.Database.BeginTransactionAsync();
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
                    Country = request.Country,
                };

                _recontext.Company.Add(company);
                await _recontext.SaveChangesAsync();

                if (!string.IsNullOrEmpty(request.userName) && !string.IsNullOrEmpty(request.password))
                {
                    _recontext.Users.Add(new XRS_Users
                    {
                        CompanyID = company.companyID,
                        UserName = request.userName,
                        Password = request.password,
                        UserType = 1,
                        CreatedDate = DateTime.Now,
                        IsActive = true
                    });
                }

                var startDate = DateTime.Today;
                var endDate = startDate.AddDays(14);

                _recontext.CompanySubscription.Add(new XRS_CompanySubscription
                {
                    CompanyId = company.companyID,
                    PlanId = 0,
                    SubscriptionDate = DateTime.Now,
                    SubscriptionStartDate = startDate,
                    SubscriptionEndDate = endDate,
                    SubscriptionDays = 14,
                    SubscriptionAmount = 0,
                    SubscriptionUserCount = 2,
                    Status = "TRIAL",
                });

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
                       .Select(d => (int?)d.DurationDays)
                        .FirstOrDefaultAsync();

                    subDto = new SubscriptionRentalSummaryDto
                    {
                        SubId = latestSub.SubId,
                        Status = latestSub.Status,
                        StartDate = latestSub.SubscriptionStartDate,
                        EndDate = latestSub.SubscriptionEndDate,
                        Amount = latestSub.SubscriptionAmount ?? 0,
                        UserCount = latestSub.SubscriptionUserCount ?? 0,
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

        //OLD CODE/// 



        //public async Task<CompanyRentalDetailDto?> GetRentalCompanyByIdAsync(int companyId)
        //{
        //    var company = await _recontext.Company
        //        .FirstOrDefaultAsync(c => c.companyID == companyId);
        //    if (company == null) return null;

        //    var latestSub = await _recontext.CompanySubscription
        //        .Where(s => s.CompanyId == companyId)
        //        .OrderByDescending(s => s.SubscriptionEndDate)
        //        .FirstOrDefaultAsync();

        //    SubscriptionRentalSummaryDto? subDto = null;

        //    if (latestSub != null)
        //    {
        //        var planName = await _recontext.SubscribePlan
        //            .Where(p => p.PlanId == latestSub.PlanId)
        //            .Select(p => p.PlanName)
        //            .FirstOrDefaultAsync();

        //        var durationDays = await _recontext.SubscribePlanDurations
        //            .Where(d =>
        //                d.PlanId == latestSub.PlanId &&
        //                d.Price == latestSub.SubscriptionAmount &&
        //                d.IsActive)
        //            .OrderBy(d => d.DurationDays)
        //            .Select(d => d.DurationDays)
        //            .FirstOrDefaultAsync();

        //        subDto = new SubscriptionRentalSummaryDto
        //        {
        //            SubId = latestSub.SubId,
        //            Status = latestSub.Status,
        //            StartDate = latestSub.SubscriptionStartDate,
        //            EndDate = latestSub.SubscriptionEndDate,
        //            Amount = latestSub.SubscriptionAmount,
        //            UserCount = latestSub.SubscriptionUserCount,
        //            PlanName = planName ?? string.Empty,
        //            DurationDays = durationDays
        //        };
        //    }

        //    return new CompanyRentalDetailDto
        //    {
        //        Company = new CompanyRentalListDto
        //        {
        //            CompanyId = company.companyID,
        //            CompanyName = company.companyName,
        //            IsActive = company.IsActive,
        //            PhoneNumber = company.phoneNumber,
        //            Address = company.address,
        //            Email = company.email,
        //            Pin = company.pin,
        //            Country = company.Country,
        //            Subscription = subDto
        //        },
        //        Settings = await _recontext.CompanySetting
        //            .Where(s => s.CompanyId == companyId)
        //            .Select(s => new CompanyRentalSettingsDto
        //            {
        //                KeyCode = s.KeyCode,
        //                Value = s.Value
        //            })
        //            .ToListAsync()
        //    };
        //}
        //public async Task<CompanyRentalDetailDto?> GetRentalCompanyByIdAsync(int companyId)
        //{
        //    try
        //    {
        //        var company = await _recontext.Company
        //            .FirstOrDefaultAsync(c => c.companyID == companyId);
        //        if (company == null) return null;

        //        var user = await _recontext.Users
        //            .Where(u => u.CompanyID == companyId)
        //            .FirstOrDefaultAsync();

        //        // ✅ Get latest NON-PENDING subscription
        //        var latestSub = await _recontext.CompanySubscription
        //            .Where(s => s.CompanyId == companyId)
        //            .OrderByDescending(s => s.SubscriptionEndDate)
        //            .FirstOrDefaultAsync();

        //        SubscriptionRentalSummaryDto? subDto = null;

        //        if (latestSub != null)
        //        {
        //            string? planName = null;
        //            int? durationDays = null;

        //            // ✅ Only lookup plan info if it's a real plan (not TRIAL)
        //            if (latestSub.PlanId != 0)
        //            {
        //                planName = await _recontext.SubscribePlan
        //                    .Where(p => p.PlanId == latestSub.PlanId)
        //                    .Select(p => p.PlanName)
        //                    .FirstOrDefaultAsync();

        //                // ✅ No price filter — just get by PlanId, prefer active
        //                durationDays = await _recontext.SubscribePlanDurations
        //                    .Where(d => d.PlanId == latestSub.PlanId)
        //                    .OrderByDescending(d => d.IsActive)
        //                    .Select(d => (int?)d.DurationDays)
        //                    .FirstOrDefaultAsync();
        //            }

        //            // ✅ Calculate real status
        //            string realStatus = latestSub.Status?.Trim().ToUpper() ?? "UNKNOWN";
        //            var currentDate = DateTime.Now;

        //            if (realStatus == "ACTIVE" && latestSub.SubscriptionEndDate < currentDate)
        //                realStatus = "EXPIRED";
        //            else if (realStatus == "TRIAL" && latestSub.SubscriptionEndDate < currentDate)
        //                realStatus = "TRIAL_EXPIRED";

        //            subDto = new SubscriptionRentalSummaryDto
        //            {
        //                SubId = latestSub.SubId,
        //                Status = realStatus,
        //                StartDate = latestSub.SubscriptionStartDate,
        //                EndDate = latestSub.SubscriptionEndDate,
        //                Amount = latestSub.SubscriptionAmount ?? 0,
        //                UserCount = latestSub.SubscriptionUserCount ?? 0,
        //                PlanName = planName ?? string.Empty,
        //                DurationDays = durationDays
        //            };
        //        }

        //        var settings = await _recontext.CompanySetting
        //            .Where(s => s.CompanyId == companyId)
        //            .Select(s => new CompanyRentalSettingsDto
        //            {
        //                KeyCode = s.KeyCode,
        //                Value = s.Value
        //            })
        //            .ToListAsync();

        //        return new CompanyRentalDetailDto
        //        {
        //            Company = new CompanyRentalListDto
        //            {
        //                CompanyId = company.companyID,
        //                CompanyName = company.companyName,
        //                IsActive = company.IsActive,
        //                PhoneNumber = company.phoneNumber,
        //                Address = company.address,
        //                Email = company.email,
        //                Pin = company.pin,
        //                Country = company.Country,

        //                Subscription = subDto
        //            },
        //            Settings = settings
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        // ✅ Log the exact error
        //        throw new Exception($"GetRentalCompanyByIdAsync failed for companyId {companyId}: {ex.Message}", ex);
        //    }
        //}




        //        public async Task<CompanyRentalDetailDto?> GetRentalCompanyByIdAsync(int companyId)
        //{
        //    try
        //    {
        //        var company = await _recontext.Company
        //            .FirstOrDefaultAsync(c => c.companyID == companyId);
        //        if (company == null) return null;

        //        var user = await _recontext.Users
        //            .Where(u => u.CompanyID == companyId)
        //            .FirstOrDefaultAsync();

        //        // ✅ Get latest subscription
        //        var latestSub = await _recontext.CompanySubscription
        //            .Where(s => s.CompanyId == companyId)
        //            .OrderByDescending(s => s.SubscriptionEndDate)
        //            .FirstOrDefaultAsync();

        //        SubscriptionRentalSummaryDto? subDto = null;

        //        if (latestSub != null)
        //        {
        //            string? planName = null;
        //            int? durationDays = null;

        //            if (latestSub.PlanId != 0)
        //            {
        //                planName = await _recontext.SubscribePlan
        //                    .Where(p => p.PlanId == latestSub.PlanId)
        //                    .Select(p => p.PlanName)
        //                    .FirstOrDefaultAsync();

        //                durationDays = await _recontext.SubscribePlanDurations
        //                    .Where(d => d.PlanId == latestSub.PlanId)
        //                    .OrderByDescending(d => d.IsActive)
        //                    .Select(d => (int?)d.DurationDays)
        //                    .FirstOrDefaultAsync();
        //            }

        //            // ✅ Calculate real status
        //            string realStatus = latestSub.Status?.Trim().ToUpper() ?? "UNKNOWN";
        //            var currentDate = DateTime.Now;

        //            if (realStatus == "ACTIVE" && latestSub.SubscriptionEndDate < currentDate)
        //                realStatus = "EXPIRED";
        //            else if (realStatus == "TRIAL" && latestSub.SubscriptionEndDate < currentDate)
        //                realStatus = "TRIAL_EXPIRED";

        //            // ✅ ADDONS
        //            var addons = await _recontext.CompanySubscriptionAddon
        //                .Where(a => a.CompanyId == companyId && a.Status == "ACTIVE")
        //                .Select(a => new SubscriptionAddonDto
        //                {
        //                    PlanId = a.PlanId,
        //                    Amount = a.Amount,
        //                    UserCount = a.DepCount,
        //                    Status = a.Status
        //                })
        //                .ToListAsync();

        //            subDto = new SubscriptionRentalSummaryDto
        //            {
        //                SubId = latestSub.SubId,
        //                Status = realStatus,
        //                StartDate = latestSub.SubscriptionStartDate,
        //                EndDate = latestSub.SubscriptionEndDate,
        //                Amount = latestSub.SubscriptionAmount ?? 0,
        //                UserCount = latestSub.SubscriptionUserCount ?? 0,
        //                PlanName = planName ?? string.Empty,
        //                DurationDays = durationDays ?? 0,
        //                Addons = addons   // ✅ ADDED
        //            };
        //        }

        //        // ✅ SETTINGS
        //        var settings = await _recontext.CompanySetting
        //            .Where(s => s.CompanyId == companyId)
        //            .Select(s => new CompanyRentalSettingsDto
        //            {
        //                KeyCode = s.KeyCode,
        //                Value = s.Value
        //            })
        //            .ToListAsync();

        //        // ✅ SUBSCRIPTION HISTORY (RENEW DETAILS)
        //        var history = await _recontext.CompanySubscription
        //            .Where(s => s.CompanyId == companyId)
        //            .OrderByDescending(s => s.SubscriptionEndDate)
        //            .Select(s => new SubscriptionHistoryDto
        //            {
        //                SubId = s.SubId,
        //                StartDate = s.SubscriptionStartDate,
        //                EndDate = s.SubscriptionEndDate,
        //                Amount = s.SubscriptionAmount ?? 0,
        //                Status = s.Status
        //            })
        //            .ToListAsync();

        //        return new CompanyRentalDetailDto
        //        {
        //            Company = new CompanyRentalListDto
        //            {
        //                CompanyId = company.companyID,
        //                CompanyName = company.companyName,
        //                IsActive = company.IsActive,
        //                PhoneNumber = company.phoneNumber,
        //                Address = company.address,
        //                Email = company.email,
        //                Pin = company.pin,
        //                Country = company.Country,
        //                Subscription = subDto
        //            },
        //            Settings = settings,
        //            SubscriptionHistory = history   // ✅ ADDED
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"GetRentalCompanyByIdAsync failed for companyId {companyId}: {ex.Message}", ex);
        //    }
        //}


        public async Task<CompanyRentalDetailDto?> GetRentalCompanyByIdAsync(int companyId)
        {
            try
            {
                var company = await _recontext.Company
                    .FirstOrDefaultAsync(c => c.companyID == companyId);
                if (company == null) return null;

                var user = await _recontext.Users
                    .Where(u => u.CompanyID == companyId)
                    .FirstOrDefaultAsync();

                // ✅ Get latest subscription
                var latestSub = await _recontext.CompanySubscription
                    .Where(s => s.CompanyId == companyId)
                    .OrderByDescending(s => s.SubscriptionEndDate)
                    .FirstOrDefaultAsync();

                SubscriptionRentalSummaryDto? subDto = null;

                if (latestSub != null)
                {
                    string? planName = null;
                    int? durationDays = null;

                    if (latestSub.PlanId != 0)
                    {
                        planName = await _recontext.SubscribePlan
                            .Where(p => p.PlanId == latestSub.PlanId)
                            .Select(p => p.PlanName)
                            .FirstOrDefaultAsync();

                        durationDays = await _recontext.SubscribePlanDurations
                            .Where(d => d.PlanId == latestSub.PlanId)
                            .OrderByDescending(d => d.IsActive)
                            .Select(d => (int?)d.DurationDays)
                            .FirstOrDefaultAsync();
                    }

                    // ✅ Calculate real status
                    string realStatus = latestSub.Status?.Trim().ToUpper() ?? "UNKNOWN";
                    var currentDate = DateTime.Now;

                    if (realStatus == "ACTIVE" && latestSub.SubscriptionEndDate < currentDate)
                        realStatus = "EXPIRED";
                    else if (realStatus == "TRIAL" && latestSub.SubscriptionEndDate < currentDate)
                        realStatus = "TRIAL_EXPIRED";

                    // ✅ Fetch addons linked to this subscription
                    var addons = await (
                        from a in _recontext.CompanySubscriptionAddon
                        join p in _recontext.SubscribePlan on a.PlanId equals p.PlanId
                        where a.CompanyId == companyId
                              && a.MainPlanId == latestSub.SubId
                              && a.Status == "ACTIVE"
                        select new SubscriptionRentalAddonDto
                        {
                            SubAddonId = a.Id,
                            AddonPlanName = p.PlanName,
                            Amount = a.Amount,
                            UserCount = a.UserCount,
                            Status = a.Status
                        }
                    ).ToListAsync();

                    var allSubscriptions = await _recontext.CompanySubscription
    .Where(s => s.CompanyId == companyId)
    .OrderByDescending(s => s.SubscriptionEndDate)
    .ToListAsync();

                    var history = new List<SubscriptionHistoryDto>();
                    foreach (var sub in allSubscriptions)
                    {
                        string subPlanName = string.Empty;
                        if (sub.PlanId != 0)
                        {
                            subPlanName = await _recontext.SubscribePlan
                                .Where(p => p.PlanId == sub.PlanId)
                                .Select(p => p.PlanName)
                                .FirstOrDefaultAsync() ?? string.Empty;
                        }

                        history.Add(new SubscriptionHistoryDto
                        {
                            SubId = sub.SubId,
                            StartDate = sub.SubscriptionStartDate,
                            EndDate = sub.SubscriptionEndDate,
                            Amount = sub.SubscriptionAmount ?? 0,
                            Status = sub.Status,
                            PlanName = subPlanName,
                            DurationDays = sub.SubscriptionDays
                        });
                    }




                    subDto = new SubscriptionRentalSummaryDto
                    {
                        SubscriptionHistory = history.Any() ? history : null  ,// Add this line
                        SubId = latestSub.SubId,
                        Status = realStatus,
                        StartDate = latestSub.SubscriptionStartDate,
                        EndDate = latestSub.SubscriptionEndDate,
                        Amount = latestSub.SubscriptionAmount ?? 0,
                        UserCount = latestSub.SubscriptionUserCount ?? 0,
                        PlanName = planName ?? string.Empty,
                        DurationDays = durationDays,
                        Addons = addons.Any() ? addons : null
                    };
                }

                // ✅ Settings
                var settings = await _recontext.CompanySetting
                    .Where(s => s.CompanyId == companyId)
                    .Select(s => new CompanyRentalSettingsDto
                    {
                        KeyCode = s.KeyCode,
                        Value = s.Value
                    })
                    .ToListAsync();

                return new CompanyRentalDetailDto
                {
                    Company = new CompanyRentalListDto
                    {
                        CompanyId = company.companyID,
                        CompanyName = company.companyName,
                        IsActive = company.IsActive,
                        PhoneNumber = company.phoneNumber,
                        Address = company.address,
                        Email = company.email,
                        Pin = company.pin,
                        Country = company.Country,
                        Logo = company.logo,
                        //UserName = user?.UserName,
                        //Password = user?.Password,
                        Subscription = subDto
                    },
                    Settings = settings
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"GetRentalCompanyByIdAsync failed for companyId {companyId}: {ex.Message}", ex);
            }
        }
        public async Task UpdateRentalCompanyAsync(UpdateRentalCompanyDto dto)
        {
            var company = await _recontext.Company
                .FirstOrDefaultAsync(c => c.companyID == dto.CompanyId);
            if (company == null) throw new Exception("Company not found");

            company.companyName = dto.CompanyName;
            company.address = dto.Address;
            company.Country = dto.Country;
            company.email = dto.Email;           // ← ADD
            company.phoneNumber = dto.PhoneNumber; // ← ADD
            company.pin = dto.Pin;               // ← ADD
            company.logo = dto.Logo;             // ← ADD

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

        #endregion

        #region TICKET

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

                // Note: Add UserName/Password to your CompanyTicketRegistrationRequestDto
                if (!string.IsNullOrEmpty(request.userName) && !string.IsNullOrEmpty(request.password))
                {
                    _ticontext.TI_Users.Add(new TI_Users
                    {
                        CompanyId = company.CompanyId,
                        UserName = request.userName,
                        Password = request.password,
                        UserType = "ADMIN",
                        UserCreatedOn = DateTime.Now,
                        UserCreatedBy = 0,
                        UserStatus = true
                    });
                }

                var startDate = DateTime.Today;
                var endDate = startDate.AddDays(14);

                _ticontext.CompanySubscription.Add(new TI_CompanySubscription
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
                });

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
            if (company == null) return null;

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
            var company = await _ticontext.Company
                .FirstOrDefaultAsync(c => c.CompanyId == dto.CompanyId);
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

            await _ticontext.SaveChangesAsync();
        }

        #endregion



    //    #region AUTHENTICATION

    //    public async Task<LoginResponseDto> AuthenticateCatalogUserAsync(LoginRequestDto request)
    //    {
    //        var user = await _cacontext.UserSettings
    //            .FirstOrDefaultAsync(u => u.UserName == request.UserName
    //                                   || u.PhoneNumber == request.UserName);

    //        if (user == null)
    //            throw new UnauthorizedAccessException("User not found.");

    //        if (user.Password != request.Password)
    //            throw new UnauthorizedAccessException("Invalid password.");

    //        if (user.Active != true)
    //            throw new UnauthorizedAccessException("User is inactive. Please contact administrator.");

    //        var company = await _cacontext.tblCompany
    //            .FirstOrDefaultAsync(c => c.CompanyId == user.CompanyId);

    //        var token = GenerateJwtToken(user, company);

    //        return new LoginResponseDto
    //        {
    //            Token = token,
    //            UserId = user.UserId,
    //            UserName = user.UserName ?? string.Empty,
    //            RoleId = user.UserRoleId,     // ✅ Use UserRoleId
    //            CompanyId = user.CompanyId,
    //            Module = "Catalog",
    //            Message = "Login successful"
    //        };
    //    }

    //    public async Task<LoginResponseDto> AuthenticateUserAsync(LoginRequestDto request)
    //    {
    //        try
    //        {
    //            return await AuthenticateCatalogUserAsync(request);
    //        }
    //        catch (UnauthorizedAccessException)
    //        {
    //            throw new UnauthorizedAccessException("Invalid username or password.");
    //        }
    //    }

    //    // ✅ CT_Company → CT_tblCompany
    //    private string GenerateJwtToken(CT_UserSetting user, CT_tblCompany? company = null)
    //    {
    //        var tokenHandler = new JwtSecurityTokenHandler();
    //        var key = Encoding.ASCII.GetBytes("YourSuperSecretKeyHereAtLeast32CharactersLong");

    //        var claims = new List<Claim>
    //{
    //    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
    //    new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
    //    new Claim("UserId", user.UserId.ToString()),
    //    new Claim("PhoneNumber", user.PhoneNumber ?? string.Empty),
    //    new Claim("Email", user.Email ?? string.Empty)
    //};

    //        // ✅ CORRECTED: Use UserRoleId instead of RoleId
    //        if (user.UserRoleId.HasValue)
    //            claims.Add(new Claim(ClaimTypes.Role, user.UserRoleId.Value.ToString()));

    //        if (user.CompanyId.HasValue)
    //            claims.Add(new Claim("CompanyId", user.CompanyId.Value.ToString()));

    //        if (company != null)
    //            claims.Add(new Claim("CompanyName", company.CompanyName ?? string.Empty));

    //        var tokenDescriptor = new SecurityTokenDescriptor
    //        {
    //            Subject = new ClaimsIdentity(claims),
    //            Expires = DateTime.UtcNow.AddDays(7),
    //            SigningCredentials = new SigningCredentials(
    //                new SymmetricSecurityKey(key),
    //                SecurityAlgorithms.HmacSha256Signature)
    //        };

    //        var token = tokenHandler.CreateToken(tokenDescriptor);
    //        return tokenHandler.WriteToken(token);
    //    }

    //    #endregion

        //#region CATALOG

        //public async Task<int> RegisterCatalogCompanyAsync(CompanyCatalogRegistrationRequestDto request)
        //{
        //    using var transaction = await _cacontext.Database.BeginTransactionAsync();
        //    try
        //    {
        //        var company = new CT_tblCompany
        //        {
        //            CompanyName = request.CompanyName,
        //            AddressLine1 = request.CompanyAddress,
        //            Phoneno1 = request.Phone1,
        //            Phoneno2 = request.Phone2,
        //            TaxRegNo = request.RegNo,
        //            Email = request.Email,
        //            Active = true,
        //            CreatedDate = DateTime.Now,
        //            ValidityDate = DateTime.Today.AddDays(14)
        //        };

        //        _cacontext.tblCompany.Add(company);
        //        await _cacontext.SaveChangesAsync();

        //        // ✅ Create Admin User (optional)
        //        if (!string.IsNullOrEmpty(request.UserName) && !string.IsNullOrEmpty(request.Password))
        //        {
        //            _cacontext.Users.Add(new CT_Users
        //            {
        //                CompanyId = company.CompanyId,
        //                UserName = request.UserName,
        //                Password = request.Password,
        //                UserType = "ADMIN",
        //                UserCreatedOn = DateTime.Now,
        //                UserCreatedBy = 0,
        //                UserStatus = true
        //            });
        //        }

        //        // ✅ Default Trial Subscription
        //        var startDate = DateTime.Today;
        //        var endDate = startDate.AddDays(14);

        //        _cacontext.CompanySubscriptions.Add(new CT_CompanySubscription
        //        {
        //            CompanyId = company.CompanyId,
        //            PlanId = 0,
        //            SubscriptionStartDate = startDate,
        //            SubscriptionEndDate = endDate,
        //            SubscriptionDays = 14,
        //            SubscriptionAmount = 0,
        //            SubscriptionUserCount = 2,
        //            Status = "TRIAL",
        //            RateType = "STANDARD",
        //            CreatedOn = DateTime.Now
        //        });

        //        // ✅ Labels
        //        if (request.Labels != null)
        //        {
        //            foreach (var label in request.Labels)
        //            {
        //                _cacontext.CompanyLabel.Add(new CT_CompanyLabel
        //                {
        //                    CompanyId = company.CompanyId,
        //                    SettingKey = label.SettingKey,
        //                    DisplayName = label.DisplayName,
        //                    DisplayNameTa = label.DisplayNameTa,
        //                    DisplayNameMa = label.DisplayNameMa,
        //                    CreatedBy = 0
        //                });
        //            }
        //        }

        //        // ✅ Replace CT_CompanySettings with CT_tblCompanySettings
        //        if (request.Settings != null)
        //        {
        //            foreach (var setting in request.Settings)
        //            {
        //                _cacontext.tblCompanySettings.Add(new CT_tblCompanySettings
        //                {
        //                    CompanyId = company.CompanyId,
        //                    KeyCode = setting.KeyCode,
        //                    Value = setting.Value,
        //                    Active = true
        //                });
        //            }
        //        }

        //        await _cacontext.SaveChangesAsync();
        //        await transaction.CommitAsync();

        //        return company.CompanyId;
        //    }
        //    catch
        //    {
        //        await transaction.RollbackAsync();
        //        throw;
        //    }
        //}

        //public async Task<List<CompanyCatalogListDto>> GetAllCatalogCompaniesAsync()
        //{
        //    var companies = await _cacontext.Company.ToListAsync();
        //    var result = new List<CompanyCatalogListDto>();

        //    foreach (var c in companies)
        //    {
        //        var latestSub = await _cacontext.CompanySubscriptions
        //            .Where(s => s.CompanyId == c.CompanyId)
        //            .OrderByDescending(s => s.SubscriptionEndDate)
        //            .FirstOrDefaultAsync();

        //        SubscriptionCatalogueSummaryDto? subDto = null;

        //        if (latestSub != null)
        //        {
        //            var planName = await _cacontext.SubscribePlan
        //                .Where(p => p.PlanId == latestSub.PlanId)
        //                .Select(p => p.PlanName)
        //                .FirstOrDefaultAsync();

        //            string realStatus = latestSub.Status?.Trim().ToUpper() ?? "UNKNOWN";

        //            if (realStatus == "ACTIVE" && latestSub.SubscriptionEndDate < DateTime.Now)
        //                realStatus = "EXPIRED";
        //            else if (realStatus == "TRIAL" && latestSub.SubscriptionEndDate < DateTime.Now)
        //                realStatus = "TRIAL_EXPIRED";

        //            subDto = new SubscriptionCatalogueSummaryDto
        //            {
        //                SubId = latestSub.SubId,
        //                Status = realStatus,
        //                StartDate = latestSub.SubscriptionStartDate,
        //                EndDate = latestSub.SubscriptionEndDate,
        //                Amount = latestSub.SubscriptionAmount,
        //                DealerAmount = latestSub.DealerAmount,
        //                RateType = latestSub.RateType,
        //                UserCount = latestSub.SubscriptionUserCount ?? 0,
        //                PlanName = planName ?? string.Empty
        //            };
        //        }

        //        result.Add(new CompanyCatalogListDto
        //        {
        //            CompanyId = c.CompanyId,
        //            CompanyName = c.CompanyName ?? string.Empty,
        //            CompanyAddress = c.CompanyAddress,
        //            CompanyPhone1 = c.CompanyPhone1,
        //            CompanyPhone2 = c.CompanyPhone2,
        //            CompanyRegNo = c.CompanyRegNo,
        //            DistrictName = c.DistrictName,
        //            StateName = c.StateName,
        //            CompanyCreatedOn = c.CompanyCreatedOn,
        //            CompanyActive = c.CompanyActive,
        //            CustomDate = c.CustomDate,
        //            Subscription = subDto
        //        });
        //    }

        //    return result;
        //}

        //public async Task<CompanyCatalogDetailDto?> GetCatalogCompanyByIdAsync(int companyId)
        //{
        //    var company = await _cacontext.Company
        //        .FirstOrDefaultAsync(c => c.CompanyId == companyId);

        //    if (company == null) return null;

        //    var latestSub = await _cacontext.CompanySubscriptions
        //        .Where(s => s.CompanyId == companyId)
        //        .OrderByDescending(s => s.SubscriptionEndDate)
        //        .FirstOrDefaultAsync();

        //    SubscriptionCatalogueSummaryDto? subDto = null;

        //    if (latestSub != null)
        //    {
        //        var planName = await _cacontext.SubscribePlan
        //            .Where(p => p.PlanId == latestSub.PlanId)
        //            .Select(p => p.PlanName)
        //            .FirstOrDefaultAsync();

        //        string realStatus = latestSub.Status?.Trim().ToUpper() ?? "UNKNOWN";

        //        if (realStatus == "ACTIVE" && latestSub.SubscriptionEndDate < DateTime.Now)
        //            realStatus = "EXPIRED";
        //        else if (realStatus == "TRIAL" && latestSub.SubscriptionEndDate < DateTime.Now)
        //            realStatus = "TRIAL_EXPIRED";

        //        var addons = await (
        //            from a in _cacontext.CompanySubscriptionAddon
        //            join p in _cacontext.SubscribePlan on a.PlanId equals p.PlanId
        //            where a.CompanyId == companyId
        //                  && a.MainPlanId == latestSub.SubId
        //                  && a.Status == "ACTIVE"
        //            select new CatalogueAddonSummaryDto
        //            {
        //                SubAddonId = a.Id,
        //                AddonPlanName = p.PlanName,
        //                Amount = a.Amount,
        //                DealerAmount = a.DealerAmount,
        //                RateType = a.RateType,
        //                UserCount = a.UserCount,
        //                Status = a.Status
        //            }
        //        ).ToListAsync();

        //        subDto = new SubscriptionCatalogueSummaryDto
        //        {
        //            SubId = latestSub.SubId,
        //            Status = realStatus,
        //            StartDate = latestSub.SubscriptionStartDate,
        //            EndDate = latestSub.SubscriptionEndDate,
        //            Amount = latestSub.SubscriptionAmount,
        //            DealerAmount = latestSub.DealerAmount,
        //            RateType = latestSub.RateType,
        //            UserCount = latestSub.SubscriptionUserCount ?? 0,
        //            PlanName = planName ?? string.Empty,
        //            Addons = addons.Any() ? addons : null
        //        };
        //    }

        //    return new CompanyCatalogDetailDto
        //    {
        //        Company = new CompanyCatalogListDto
        //        {
        //            CompanyId = company.CompanyId,
        //            CompanyName = company.CompanyName ?? string.Empty,
        //            CompanyAddress = company.CompanyAddress,
        //            CompanyPhone1 = company.CompanyPhone1,
        //            CompanyPhone2 = company.CompanyPhone2,
        //            CompanyRegNo = company.CompanyRegNo,
        //            DistrictName = company.DistrictName,
        //            StateName = company.StateName,
        //            CompanyCreatedOn = company.CompanyCreatedOn,
        //            CompanyActive = company.CompanyActive,
        //            CustomDate = company.CustomDate,
        //            Subscription = subDto
        //        },
        //        // ✅ Replace CompanySetting with tblCompanySettings
        //        Settings = await _cacontext.tblCompanySettings
        //        .Where(s => s.CompanyId == companyId && s.Active == true)
        //         .Select(s => new CompanyCatalogSettingDto
        //        {
        //           KeyCode = s.KeyCode,
        //             Value = s.Value
        //         })
        //        .ToListAsync(),

        //        Labels = await _cacontext.CompanyLabel
        //            .Where(l => l.CompanyId == companyId)
        //            .Select(l => new CompanyCatalogLabelDto
        //            {
        //                SettingKey = l.SettingKey,
        //                DisplayName = l.DisplayName,
        //                DisplayNameTa = l.DisplayNameTa,
        //                DisplayNameMa = l.DisplayNameMa
        //            })
        //            .ToListAsync()
        //    };
        //}

        //#endregion


        #region CATALOG

        public async Task<int> RegisterCatalogCompanyAsync(CompanyCatalogRegistrationRequestDto request)
        {
            using var transaction = await _cacontext.Database.BeginTransactionAsync();
            try
            {
                var company = new CT_tblCompany
                {
                    CompanyName = request.CompanyName,
                    AddressLine1 = request.CompanyAddress,
                    AddressLine2 = request.AddressLine2,
                    Phoneno1 = request.Phone1,
                    Phoneno2 = request.Phone2,
                    Latitude = request.Latitude,
                    Longitude = request.Longitude,
                    Email = request.Email,
                    Pincode = request.Pincode,
                    Website = request.Website,
                    DecimalValue = request.DecimalValue,
                    TaxType = request.TaxType,
                    Currency = request.Currency,
                    CurrencySymbol = request.CurrencySymbol,
                    MajorCurrency = request.MajorCurrency,
                    MinorCurrency = request.MinorCurrency,
                    OrderNoPrefix = request.OrderNoPrefix,
                    OrderNoSuffix = request.OrderNoSuffix,
                    BranchLimit = request.BranchLimit,
                    CompanyBrand = request.CompanyBrand,
                    CustomerCareNo = request.CustomerCareNo,
                    CustomerCareEmail = request.CustomerCareEmail,
                    FooterMessage = request.FooterMessage,
                    Logo = request.Logo,
                    TaxRegNo = request.RegNo,
                    SoldBy = request.SoldBy,
                    Active = true,
                    CreatedDate = DateTime.Now,
                    ValidityDate = DateTime.Today.AddDays(14)
                };

                _cacontext.tblCompany.Add(company);
                await _cacontext.SaveChangesAsync();

                // ✅ Store user in tblUserSettings (CT_UserSetting)
                if (!string.IsNullOrEmpty(request.UserName) && !string.IsNullOrEmpty(request.Password))
                {
                    _cacontext.UserSettings.Add(new CT_UserSetting  // Changed from Users
                    {
                        CompanyId = company.CompanyId,
                        UserName = request.UserName,
                        Password = request.Password,
                        PhoneNumber = request.Phone1,
                        Email = request.Email,
                        UserRoleId = 1,  // Admin role
                        Active = true,
                        Designation = "Admin",
                        Address = request.CompanyAddress,
                        DOB = null,
                        Image = null,
                        DefaultBranch = null
                    });
                }

                var startDate = DateTime.Today;
                var endDate = startDate.AddDays(14);

                _cacontext.CompanySubscriptions.Add(new CT_CompanySubscription
                {
                    CompanyId = company.CompanyId,
                    PlanId = 0,
                    SubscriptionStartDate = startDate,
                    SubscriptionEndDate = endDate,
                    SubscriptionDays = 14,
                    SubscriptionAmount = 0,
                    SubscriptionUserCount = 2,
                    Status = "TRIAL",
                    RateType = "STANDARD",
                    CreatedOn = DateTime.Now
                });

                if (request.Labels != null)
                {
                    foreach (var label in request.Labels)
                    {
                        _cacontext.CompanyLabel.Add(new CT_CompanyLabel
                        {
                            CompanyId = company.CompanyId,
                            SettingKey = label.SettingKey,
                            DisplayName = label.DisplayName,
                            DisplayNameTa = label.DisplayNameTa,
                            DisplayNameMa = label.DisplayNameMa,
                            CreatedBy = 0
                        });
                    }
                }

                if (request.Settings != null)
                {
                    foreach (var setting in request.Settings)
                    {
                        _cacontext.tblCompanySettings.Add(new CT_tblCompanySettings
                        {
                            CompanyId = company.CompanyId,
                            KeyCode = setting.KeyCode,
                            Value = setting.Value,
                            Active = true
                        });
                    }
                }

                await _cacontext.SaveChangesAsync();
                await transaction.CommitAsync();
                return company.CompanyId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<CompanyCatalogListDto>> GetAllCatalogCompaniesAsync()
        {
            // ✅ Use tblCompany instead of Company
            var companies = await _cacontext.tblCompany.ToListAsync();
            var result = new List<CompanyCatalogListDto>();

            foreach (var c in companies)
            {
                var latestSub = await _cacontext.CompanySubscriptions
                    .Where(s => s.CompanyId == c.CompanyId)
                    .OrderByDescending(s => s.SubscriptionEndDate)
                    .FirstOrDefaultAsync();

                SubscriptionCatalogueSummaryDto? subDto = null;

                if (latestSub != null)
                {
                    var planName = await _cacontext.SubscribePlan
                        .Where(p => p.PlanId == latestSub.PlanId)
                        .Select(p => p.PlanName)
                        .FirstOrDefaultAsync();

                    string realStatus = latestSub.Status?.Trim().ToUpper() ?? "UNKNOWN";

                    if (realStatus == "ACTIVE" && latestSub.SubscriptionEndDate < DateTime.Now)
                        realStatus = "EXPIRED";
                    else if (realStatus == "TRIAL" && latestSub.SubscriptionEndDate < DateTime.Now)
                        realStatus = "TRIAL_EXPIRED";

                    subDto = new SubscriptionCatalogueSummaryDto
                    {
                        SubId = latestSub.SubId,
                        Status = realStatus,
                        StartDate = latestSub.SubscriptionStartDate,
                        EndDate = latestSub.SubscriptionEndDate,
                        Amount = latestSub.SubscriptionAmount,
                        DealerAmount = latestSub.DealerAmount,
                        RateType = latestSub.RateType,
                        UserCount = latestSub.SubscriptionUserCount ?? 0,
                        PlanName = planName ?? string.Empty
                    };
                }

                // ✅ Map tblCompany fields to DTO
                result.Add(new CompanyCatalogListDto
                {
                    CompanyId = c.CompanyId,
                    CompanyName = c.CompanyName,
                    CompanyAddress = c.AddressLine1,    // ✅ AddressLine1
                    CompanyPhone1 = c.Phoneno1,         // ✅ Phoneno1
                    CompanyPhone2 = c.Phoneno2,         // ✅ Phoneno2
                    CompanyRegNo = c.TaxRegNo,          // ✅ TaxRegNo
                    CompanyActive = c.Active,           // ✅ Active
                    CompanyCreatedOn = c.CreatedDate,   // ✅ CreatedDate
                    CompanyLogo = c.Logo,
                    AddressLine2 = c.AddressLine2,
                    Email = c.Email,
                    Latitude = c.Latitude,
                    Longitude = c.Longitude,
                    Pincode = c.Pincode,
                    Website = c.Website,
                    DecimalValue = c.DecimalValue,
                    TaxType = c.TaxType,
                    Currency = c.Currency,
                    CurrencySymbol = c.CurrencySymbol,
                    MajorCurrency = c.MajorCurrency,
                    MinorCurrency = c.MinorCurrency,
                    OrderNoPrefix = c.OrderNoPrefix,
                    OrderNoSuffix = c.OrderNoSuffix,
                    BranchLimit = c.BranchLimit,
                    CompanyBrand = c.CompanyBrand,
                    CustomerCareNo = c.CustomerCareNo,
                    CustomerCareEmail = c.CustomerCareEmail,
                    FooterMessage = c.FooterMessage,
                    SoldBy = c.SoldBy,
                    Subscription = subDto
                });
            }

            return result;
        }

        public async Task<CompanyCatalogDetailDto?> GetCatalogCompanyByIdAsync(int companyId)
        {
            // ✅ Use tblCompany instead of Company
            var company = await _cacontext.tblCompany
                .FirstOrDefaultAsync(c => c.CompanyId == companyId);

            if (company == null) return null;

            var latestSub = await _cacontext.CompanySubscriptions
                .Where(s => s.CompanyId == companyId)
                .OrderByDescending(s => s.SubscriptionEndDate)
                .FirstOrDefaultAsync();

            SubscriptionCatalogueSummaryDto? subDto = null;

            if (latestSub != null)
            {
                var planName = await _cacontext.SubscribePlan
                    .Where(p => p.PlanId == latestSub.PlanId)
                    .Select(p => p.PlanName)
                    .FirstOrDefaultAsync();

                string realStatus = latestSub.Status?.Trim().ToUpper() ?? "UNKNOWN";

                if (realStatus == "ACTIVE" && latestSub.SubscriptionEndDate < DateTime.Now)
                    realStatus = "EXPIRED";
                else if (realStatus == "TRIAL" && latestSub.SubscriptionEndDate < DateTime.Now)
                    realStatus = "TRIAL_EXPIRED";

                var addons = await (
                    from a in _cacontext.CompanySubscriptionAddon
                    join p in _cacontext.SubscribePlan on a.PlanId equals p.PlanId
                    where a.CompanyId == companyId
                          && a.MainPlanId == latestSub.SubId
                          && a.Status == "ACTIVE"
                    select new CatalogueAddonSummaryDto
                    {
                        SubAddonId = a.Id,
                        AddonPlanName = p.PlanName,
                        Amount = a.Amount,
                        DealerAmount = a.DealerAmount,
                        RateType = a.RateType,
                        UserCount = a.UserCount,
                        Status = a.Status
                    }
                ).ToListAsync();

                subDto = new SubscriptionCatalogueSummaryDto
                {
                    SubId = latestSub.SubId,
                    Status = realStatus,
                    StartDate = latestSub.SubscriptionStartDate,
                    EndDate = latestSub.SubscriptionEndDate,
                    Amount = latestSub.SubscriptionAmount,
                    DealerAmount = latestSub.DealerAmount,
                    RateType = latestSub.RateType,
                    UserCount = latestSub.SubscriptionUserCount ?? 0,
                    PlanName = planName ?? string.Empty,
                    Addons = addons.Any() ? addons : null
                };
            }

            return new CompanyCatalogDetailDto
            {
                // ✅ Map tblCompany fields to DTO
                Company = new CompanyCatalogListDto
                {
                    CompanyId = company.CompanyId,
                    CompanyName = company.CompanyName,
                    CompanyAddress = company.AddressLine1,
                    CompanyPhone1 = company.Phoneno1,
                    CompanyPhone2 = company.Phoneno2,
                    CompanyRegNo = company.TaxRegNo,
                    CompanyActive = company.Active,
                    CompanyCreatedOn = company.CreatedDate,
                    CompanyLogo = company.Logo,
                    AddressLine2 = company.AddressLine2,
                    Email = company.Email,
                    Latitude = company.Latitude,
                    Longitude = company.Longitude,
                    Pincode = company.Pincode,
                    Website = company.Website,
                    DecimalValue = company.DecimalValue,
                    TaxType = company.TaxType,
                    Currency = company.Currency,
                    CurrencySymbol = company.CurrencySymbol,
                    MajorCurrency = company.MajorCurrency,
                    MinorCurrency = company.MinorCurrency,
                    OrderNoPrefix = company.OrderNoPrefix,
                    OrderNoSuffix = company.OrderNoSuffix,
                    BranchLimit = company.BranchLimit,
                    CompanyBrand = company.CompanyBrand,
                    CustomerCareNo = company.CustomerCareNo,
                    CustomerCareEmail = company.CustomerCareEmail,
                    FooterMessage = company.FooterMessage,
                    SoldBy = company.SoldBy,
                    Subscription = subDto
                },
                // ✅ Use tblCompanySettings
                Settings = await _cacontext.tblCompanySettings
                    .Where(s => s.CompanyId == companyId && s.Active == true)
                    .Select(s => new CompanyCatalogSettingDto
                    {
                        KeyCode = s.KeyCode,
                        Value = s.Value
                    })
                    .ToListAsync(),

                Labels = await _cacontext.CompanyLabel
                    .Where(l => l.CompanyId == companyId)
                    .Select(l => new CompanyCatalogLabelDto
                    {
                        SettingKey = l.SettingKey,
                        DisplayName = l.DisplayName,
                        DisplayNameTa = l.DisplayNameTa,
                        DisplayNameMa = l.DisplayNameMa
                    })
                    .ToListAsync()
            };
        }
        #endregion









    }
}

