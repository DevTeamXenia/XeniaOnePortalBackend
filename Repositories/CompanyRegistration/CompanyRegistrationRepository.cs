using Microsoft.EntityFrameworkCore;
using XeniaRegistrationBackend.Dtos;
using XeniaRegistrationBackend.Models;
using XeniaTempleBackend.Models;

namespace XeniaRegistrationBackend.Repositories.CompanyRegistration
{
    public class CompanyRegistrationRepository : ICompanyRegistrationRepository
    {
        private readonly TempleDbContext _context;

        public CompanyRegistrationRepository(TempleDbContext context)
        {
            _context = context;
        }

        public async Task<int> RegisterCompanyAsync( CompanyRegistrationRequestDto request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

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

                _context.Company.Add(company);
                await _context.SaveChangesAsync();

           
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

                _context.CompanySubscriptions.Add(trialSubscription);


                foreach (var label in request.Labels)
                {
                    _context.CompanyLabel.Add(new TK_CompanyLabel
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
                    _context.CompanySetting.Add(new TK_CompanySettings
                    {
                        CompanyId = company.CompanyId,
                        KeyCode = setting.KeyCode,
                        Value = setting.Value
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return company.CompanyId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<CompanyListDto>> GetAllCompaniesAsync()
        {
            var companies = await _context.Company.ToListAsync();

            var result = new List<CompanyListDto>();

            foreach (var c in companies)
            {
                var latestSub = await _context.CompanySubscriptions
                    .Where(s => s.CompanyId == c.CompanyId)
                    .OrderByDescending(s => s.SubscriptionEndDate)
                    .FirstOrDefaultAsync();

                SubscriptionSummaryDto? subDto = null;

                if (latestSub != null)
                {
                    var addons = await _context.CompanySubscriptionAddon
                        .Where(a => a.CompanyId == c.CompanyId && a.PlanId == latestSub.PlanId)
                        .Select(a => new SubscriptionAddonDto
                        {
                            SubAddonId = a.SubAddonId,
                            Amount = a.Amount,
                            UserCount = a.UserCount
                        }).ToListAsync();

                    subDto = new SubscriptionSummaryDto
                    {
                        SubId = latestSub.SubId,
                        Status = latestSub.Status,
                        StartDate = latestSub.SubscriptionStartDate,
                        EndDate = latestSub.SubscriptionEndDate,
                        Amount = latestSub.SubscriptionAmount,
                        UserCount = latestSub.subscriptionUserCount,
                        Addons = addons.Any() ? addons : null
                    };
                }

                result.Add(new CompanyListDto
                {
                    CompanyId = c.CompanyId,
                    CompanyName = c.CompanyName,
                    CompanyType = c.CompanyType,
                    District = c.DistrictName,
                    State = c.StateName,
                    Subscription = subDto
                });
            }

            return result;
        }

        public async Task<CompanyDetailDto?> GetCompanyByIdAsync(int companyId)
        {
            var company = await _context.Company.FirstOrDefaultAsync(c => c.CompanyId == companyId);
            if (company == null) return null;

            var subscription = await _context.CompanySubscriptions
                .Where(s => s.CompanyId == companyId)
                .OrderByDescending(s => s.SubscriptionEndDate)
                .FirstOrDefaultAsync();

            SubscriptionSummaryDto? subDto = null;

            if (subscription != null)
            {
                var addons = await _context.CompanySubscriptionAddon
                    .Where(a => a.CompanyId == companyId && a.PlanId == subscription.PlanId)
                    .Select(a => new SubscriptionAddonDto
                    {
                        SubAddonId = a.SubAddonId,
                        Amount = a.Amount,
                        UserCount = a.UserCount
                    }).ToListAsync();

                subDto = new SubscriptionSummaryDto
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

            return new CompanyDetailDto
            {
                Company = new CompanyListDto
                {
                    CompanyId = company.CompanyId,
                    CompanyName = company.CompanyName,
                    CompanyType = company.CompanyType,
                    District = company.DistrictName,
                    State = company.StateName,
                    Subscription = subDto
                },
                Settings = await _context.CompanySetting
                    .Where(s => s.CompanyId == companyId)
                    .Select(s => new CompanySettingDto
                    {
                        KeyCode = s.KeyCode,
                        Value = s.Value
                    }).ToListAsync(),

                Labels = await _context.CompanyLabel
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

        public async Task UpdateCompanyAsync(UpdateCompanyDto dto)
        {
            var company = await _context.Company.FirstOrDefaultAsync(c => c.CompanyId == dto.CompanyId);
            if (company == null) throw new Exception("Company not found");

            company.CompanyName = dto.CompanyName;
            company.CompanyAddress = dto.Address;

            _context.CompanySetting.RemoveRange(
                _context.CompanySetting.Where(s => s.CompanyId == dto.CompanyId));

            _context.CompanyLabel.RemoveRange(
                _context.CompanyLabel.Where(l => l.CompanyId == dto.CompanyId));

            await _context.SaveChangesAsync();

            _context.CompanySetting.AddRange(dto.Settings.Select(s => new TK_CompanySettings
            {
                CompanyId = dto.CompanyId,
                KeyCode = s.KeyCode,
                Value = s.Value
            }));

            _context.CompanyLabel.AddRange(dto.Labels.Select(l => new TK_CompanyLabel
            {
                CompanyId = dto.CompanyId,
                SettingKey = l.SettingKey,
                DisplayName = l.DisplayName,
                DisplayNameTa = l.DisplayNameTa,
                DisplayNameMa = l.DisplayNameMa
            }));

            await _context.SaveChangesAsync();
        }


    }

}
