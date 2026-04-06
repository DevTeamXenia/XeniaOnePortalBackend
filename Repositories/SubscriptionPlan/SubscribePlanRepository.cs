namespace XeniaRegistrationBackend.Repositories.SubscriptionPlan
{
    using Microsoft.EntityFrameworkCore;
    using XeniaRegistrationBackend.Dtos;
    using XeniaRegistrationBackend.Models;
    using XeniaRegistrationBackend.Models.Rental;
    using XeniaRegistrationBackend.Models.Temple;
    using XeniaRegistrationBackend.Models.Ticket;
    using XeniaRegistrationBackend.Models.Token;
    using XeniaRegistrationBackend.Models.Token.XeniaTempleBackend.Models;

    public class SubscribePlanRepository : ISubscribePlanRepository
    {
        private readonly TempleDbContext _tecontext;
        private readonly TokenDbContext  _tocontext;
        private readonly RentalDbContext _recontext;
        private readonly TicketDbContext _ticontext;

        public SubscribePlanRepository(TempleDbContext tecontext, TokenDbContext tocontext, RentalDbContext recontext, TicketDbContext ticontext)
        {
            _tecontext = tecontext;
            _tocontext = tocontext;
            _recontext = recontext;
            _ticontext = ticontext;
        }


        #region TEMPLE

        public async Task<int> CreateTempleSubscribePlanAsync(SubscribeTemplePlanRequestDto request)
        {
            using var tx = await _tecontext.Database.BeginTransactionAsync();

            var plan = new TK_SubscribePlan
            {
                PlanName = request.PlanName,
                PlanDescription = request.PlanDescription,
                PlanUsers = request.PlanUsers,
                PlanIsAddOn = false,
                PlanActive = true
            };

            _tecontext.SubscribePlan.Add(plan);
            await _tecontext.SaveChangesAsync();

            var durations = request.Durations.Select(d => new TK_SubscribePlanDuration
            {
                PlanId = plan.PlanId,
                DurationDays = d.DurationDays,
                Price = d.Price,
                IsActive = true
            });

            _tecontext.SubscribePlanDuration.AddRange(durations);

            await _tecontext.SaveChangesAsync();
            await tx.CommitAsync();

            return plan.PlanId;
        }

        public async Task<bool> CreateTempleSubscribeUpdateAsync( int planId, SubscribeTemplePlanRequestDto request)
        {
            using var tx = await _tecontext.Database.BeginTransactionAsync();

            var plan = await _tecontext.SubscribePlan
                .Include(p => p.PlanDurations)
                .FirstOrDefaultAsync(p => p.PlanId == planId);

            if (plan == null) return false;

            plan.PlanName = request.PlanName;
            plan.PlanDescription = request.PlanDescription;
            plan.PlanUsers = request.PlanUsers;
            plan.PlanIsAddOn = request.planIsAddOn;
            plan.PlanActive = request.PlanActive;
            plan.PlanModifiedBy = 0;
            plan.PlanModifiedOn = DateTime.Now;

            foreach (var d in plan.PlanDurations)
            {
                d.IsActive = false;
            }

            var newDurations = request.Durations.Select(d => new TK_SubscribePlanDuration
            {
                PlanId = plan.PlanId,
                DurationDays = d.DurationDays,
                Price = d.Price,
                IsActive = true,
                CreatedOn = DateTime.Now
            }).ToList();

            await _tecontext.SubscribePlanDuration.AddRangeAsync(newDurations);

            await _tecontext.SaveChangesAsync();
            await tx.CommitAsync();

            return true;
        }

        public async Task<IEnumerable<SubscribeTemplePlanResponseDto>> GetAllTempleSubscriptionPlanAsync()
        {
            return await _tecontext.SubscribePlan
                .Include(p => p.PlanDurations)
                .Select(p => new SubscribeTemplePlanResponseDto
                {
                    PlanId = p.PlanId,
                    PlanName = p.PlanName,
                    PlanDescription = p.PlanDescription,
                    PlanIsAddOn = p.PlanIsAddOn,
                    PlanUsers = p.PlanUsers,
                    PlanActive = p.PlanActive,
                    Durations = p.PlanDurations
                        .Where(d => d.IsActive)
                        .OrderBy(d => d.DurationDays)
                        .Select(d => new PlanDurationResponseDto
                        {
                            PlanDurationId = d.PlanDurationId,
                            DurationDays = d.DurationDays,
                            Price = d.Price
                        })
                        .ToList()
                })
                .ToListAsync();
        }

        public async Task<SubscribeTemplePlanResponseDto?>GetSubscriptionTemplePlanByIdAsync(int planId)
        {
            return await _tecontext.SubscribePlan
                .Include(p => p.PlanDurations)
                .Where(p => p.PlanId == planId)
                .Select(p => new SubscribeTemplePlanResponseDto
                {
                    PlanId = p.PlanId,
                    PlanName = p.PlanName,
                    PlanDescription = p.PlanDescription,
                    PlanUsers = p.PlanUsers,
                    PlanIsAddOn = p.PlanIsAddOn,
                    PlanActive = p.PlanActive,
                    Durations = p.PlanDurations
                        .Where(d => d.IsActive)
                        .OrderBy(d => d.DurationDays)
                        .Select(d => new PlanDurationResponseDto
                        {
                            PlanDurationId = d.PlanDurationId,
                            DurationDays = d.DurationDays,
                            Price = d.Price
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> CreateTempleSubscriptionAsync( CompanyTempleSubscriptionCreateDto dto)
        {
            using var transaction = await _tecontext.Database.BeginTransactionAsync();

            try
            {
                var activeSubscription = await _tecontext.CompanySubscriptions
                    .FirstOrDefaultAsync(s =>
                        s.CompanyId == dto.CompanyId &&
                        s.Status == "ACTIVE");

                if (activeSubscription != null)
                {
                    activeSubscription.Status = "EXPIRED";
                    activeSubscription.SubscriptionEndDate = DateTime.Now;

                    var activeAddons = await _tecontext.CompanySubscriptionAddon
                        .Where(a =>
                            a.CompanyId == dto.CompanyId &&
                            a.Status == "ACTIVE")
                        .ToListAsync();

                    foreach (var addon in activeAddons)
                        addon.Status = "EXPIRED";

                    await _tecontext.SaveChangesAsync();
                }

  
                var plan = await _tecontext.SubscribePlan
                    .FirstOrDefaultAsync(p =>
                        p.PlanId == dto.PlanId &&
                        p.PlanActive);

                if (plan == null)
                    throw new Exception("Invalid or inactive plan");

   
                var duration = await _tecontext.SubscribePlanDuration
                    .FirstOrDefaultAsync(d =>
                        d.PlanDurationId == dto.PlanDurationId &&
                        d.PlanId == dto.PlanId &&
                        d.IsActive);

                if (duration == null)
                    throw new Exception("Invalid plan duration");

                var startDate = DateTime.Now;
                var endDate = startDate
                    .AddDays(duration.DurationDays)
                    .AddTicks(-1);

    
                var subscription = new TK_CompanySubscription
                {
                    PlanId = plan.PlanId,
                    CompanyId = dto.CompanyId,

                    SubscriptionStartDate = startDate,
                    SubscriptionEndDate = endDate,

                    SubscriptionDays = duration.DurationDays,
                    SubscriptionAmount = duration.Price,
                    subscriptionUserCount = plan.PlanUsers,

                    Status = "ACTIVE"
                };

                _tecontext.CompanySubscriptions.Add(subscription);
                await _tecontext.SaveChangesAsync();


                if (dto.Addons?.Any() == true)
                {
                    var addons = dto.Addons.Select(a => new TK_CompanySubscriptionAddon
                    {
                        MainPlanId = subscription.SubId,
                        PlanId = a.PlanId,
                        CompanyId = dto.CompanyId,
                        Amount = a.Amount,
                        UserCount = a.UserCount,
                        Status = "ACTIVE"
                    });

                    _tecontext.CompanySubscriptionAddon.AddRange(addons);
                    await _tecontext.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return subscription.SubId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<int> CreateTempleAddonAsync(CompanyTempleSubscriptionAddonCreateDto dto)
        {
            var addon = new TK_CompanySubscriptionAddon
            {
                PlanId = dto.PlanId,
                CompanyId = dto.CompanyId,
                Amount = dto.Amount,
                UserCount = dto.UserCount
            };

            _tecontext.CompanySubscriptionAddon.Add(addon);
            await _tecontext.SaveChangesAsync();

            return addon.SubAddonId;
        }


        #endregion

        #region TOKEN

        public async Task<int> CreateTokenSubscribePlanAsync(SubscribeTokenPlanRequestDto request)
        {
            var plan = new xtm_SubscribePlan
            {
                PlanName = request.PlanName,
                PlanDescription = request.PlanDescription,
                PlanDep = request.PlanDeps,
                PlanUsers = request.PlanUsers,
                PlanIsAddOn = request.planIsAddOn,
                PlanActive = request.PlanActive,
                PlanCreatedOn = DateTime.UtcNow,
                PlanDurations = request.Durations.Select(d => new xtm_SubscribePlanDuration
                {
                    DurationDays = d.DurationDays,
                    Price = d.Price,
                    IsActive = d.IsActive,
                    CreatedOn = DateTime.UtcNow
                }).ToList()
            };

            _tocontext.SubscribePlans.Add(plan);
            await _tocontext.SaveChangesAsync();

            return plan.PlanId;
        }

        public async Task<bool> CreateTokenSubscribeUpdateAsync(int id, SubscribeTokenPlanRequestDto request)
        {
            var plan = await _tocontext.SubscribePlans
                .Include(p => p.PlanDurations)
                .FirstOrDefaultAsync(p => p.PlanId == id);

            if (plan == null) return false;

            plan.PlanName = request.PlanName;
            plan.PlanDescription = request.PlanDescription;
            plan.PlanDep = request.PlanDeps;
            plan.PlanUsers = request.PlanUsers;
            plan.PlanIsAddOn = request.planIsAddOn;
            plan.PlanActive = request.PlanActive;
            plan.PlanModifiedOn = DateTime.UtcNow;
            _tocontext.SubscribePlanDuration.RemoveRange(plan.PlanDurations);

            plan.PlanDurations = request.Durations.Select(d => new xtm_SubscribePlanDuration
            {
                DurationDays = d.DurationDays,
                Price = d.Price,
                IsActive = d.IsActive,
                CreatedOn = DateTime.UtcNow
            }).ToList();

            await _tocontext.SaveChangesAsync();
            return true;
        }

        public async Task<List<SubscribeTokenPlanResponseDto>>GetAllTokenSubscriptionPlanAsync()
        {
            return await _tocontext.SubscribePlans
                .Include(p => p.PlanDurations)
                .Select(p => new SubscribeTokenPlanResponseDto
                {
                    PlanId = p.PlanId,
                    PlanName = p.PlanName,
                    PlanDescription = p.PlanDescription,
                    PlanDeps = p.PlanDep,
                    PlanUsers = p.PlanUsers,
                    PlanIsAddOn = p.PlanIsAddOn,
                    PlanActive = p.PlanActive,

                    Durations = p.PlanDurations.Select(d =>
                        new SubscribeTokenPlanDurationResponseDto
                        {
                            PlanDurationId = d.PlanDurationId,
                            DurationDays = d.DurationDays,
                            Price = d.Price,
                            IsActive = d.IsActive
                        }).ToList()
                })
                .ToListAsync();
        }

        public async Task<SubscribeTokenPlanResponseDto?> GetSubscriptionTokenPlanByIdAsync(int id)
        {
            return await _tocontext.SubscribePlans
                .Include(p => p.PlanDurations)
                .Where(p => p.PlanId == id)
                .Select(p => new SubscribeTokenPlanResponseDto
                {
                    PlanId = p.PlanId,
                    PlanName = p.PlanName,
                    PlanDescription = p.PlanDescription,
                    PlanDeps = p.PlanDep,
                    PlanUsers = p.PlanUsers,
                    PlanIsAddOn = p.PlanIsAddOn,
                    PlanActive = p.PlanActive,

                    Durations = p.PlanDurations.Select(d =>
                        new SubscribeTokenPlanDurationResponseDto
                        {
                            PlanDurationId = d.PlanDurationId,
                            DurationDays = d.DurationDays,
                            Price = d.Price,
                            IsActive = d.IsActive
                        }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> CreateTokenSubscriptionAsync( CompanyTokenSubscriptionCreateDto dto)
        {
            using var transaction = await _tocontext.Database.BeginTransactionAsync();

            try
            {
                var activeSubscription = await _tocontext.CompanySubscriptions
                    .FirstOrDefaultAsync(s =>
                        s.CompanyId == dto.CompanyId &&
                        s.Status == "ACTIVE");

                if (activeSubscription != null)
                {
                    activeSubscription.Status = "EXPIRED";
                    activeSubscription.SubscriptionEndDate = DateTime.Now;

                    var activeAddons = await _tocontext.CompanySubscriptionAddon
                        .Where(a =>
                            a.CompanyId == dto.CompanyId &&
                            a.Status == "ACTIVE")
                        .ToListAsync();

                    foreach (var addon in activeAddons)
                        addon.Status = "EXPIRED";

                    await _tocontext.SaveChangesAsync();
                }

    
                var plan = await _tocontext.SubscribePlans
                    .FirstOrDefaultAsync(p =>
                        p.PlanId == dto.PlanId &&
                        p.PlanActive);

                if (plan == null)
                    throw new Exception("Invalid or inactive plan");

    
                var duration = await _tocontext.SubscribePlanDuration
                    .FirstOrDefaultAsync(d =>
                        d.PlanDurationId == dto.PlanDurationId &&
                        d.PlanId == dto.PlanId &&
                        d.IsActive);

                if (duration == null)
                    throw new Exception("Invalid or inactive plan duration");

                var startDate = DateTime.Now;
                var endDate = startDate
                    .AddDays(duration.DurationDays)
                    .AddTicks(-1);

                var subscription = new xtm_CompanySubscription
                {
                    PlanId = plan.PlanId,
                    CompanyId = dto.CompanyId,
                    SubscriptionStartDate = startDate,
                    SubscriptionEndDate = endDate,
                    SubscriptionDays = duration.DurationDays,
                    SubscriptionAmount = duration.Price,
                    SubscriptionDepCount = plan.PlanDep,
                    Status = "ACTIVE"
                };

                _tocontext.CompanySubscriptions.Add(subscription);
                await _tocontext.SaveChangesAsync();

                if (dto.Addons != null && dto.Addons.Any())
                {
                    var addons = dto.Addons.Select(a => new xtm_CompanySubscriptionAddon
                    {
                        MainPlanId = plan.PlanId,
                        PlanId = a.PlanId,
                        CompanyId = dto.CompanyId,
                        Amount = a.Amount,
                        DepCount = a.UserCount,
                        Status = "ACTIVE"
                    });

                    _tocontext.CompanySubscriptionAddon.AddRange(addons);
                    await _tocontext.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return subscription.SubId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<int> CreateTokenAddonAsync(CompanyTokenSubscriptionAddonCreateDto dto)
        {
            var addon = new TK_CompanySubscriptionAddon
            {
                PlanId = dto.PlanId,
                CompanyId = dto.CompanyId,
                Amount = dto.Amount,
                UserCount = dto.UserCount
            };

            _tecontext.CompanySubscriptionAddon.Add(addon);
            await _tecontext.SaveChangesAsync();

            return addon.SubAddonId;
        }

        #endregion

        #region RENTAL

        public async Task<bool> CreateRentalSubscribeUpdateAsync( int planId, SubscribeRentalPlanRequestDto request)
        {
            var plan = await _recontext.SubscribePlan
                .Include(p => p.PlanDurations)
                .FirstOrDefaultAsync(p => p.PlanId == planId);

            if (plan == null)
                return false;

            plan.PlanName = request.PlanName;
            plan.PlanDescription = request.PlanDescription;
            plan.PlanActive = request.PlanActive;
            plan.PlanUsers = request.PlanUsers;  // ADD THIS
            plan.PlanIsAddOn = request.PlanIsAddOn;

            _recontext.SubscribePlanDurations.RemoveRange(plan.PlanDurations);
     
            plan.PlanDurations.Add(new XRS_SubscribePlanDuration
            {
                DurationDays = request.PlanDurationDays,
                Price = request.PlanPrice,
                IsActive = true,
                CreatedOn = DateTime.UtcNow
            });

            await _recontext.SaveChangesAsync();
            return true;
        }

        public async Task<int> CreateRentalSubscribePlanAsync( SubscribeRentalPlanRequestDto request)
        {
            var plan = new XRS_SubscribePlan
            {
                PlanName = request.PlanName,
                PlanDescription = request.PlanDescription,
                PlanActive = request.PlanActive,
                 PlanUsers = request.PlanUsers  ,
                PlanIsAddOn = request.PlanIsAddOn   // ADD THIS// ADD THIS
            };

            _recontext.SubscribePlan.Add(plan);
            await _recontext.SaveChangesAsync();

            var duration = new XRS_SubscribePlanDuration
            {
                PlanId = plan.PlanId,
                DurationDays = request.PlanDurationDays,
                Price = request.PlanPrice,
                IsActive = true,
                CreatedOn = DateTime.UtcNow,   // ← this was missing, causing the crash
               

            };

            _recontext.SubscribePlanDurations.Add(duration);
            await _recontext.SaveChangesAsync();

            return plan.PlanId;
        }

        public async Task<IEnumerable<SubscribeRentalPlanResponseDto>> GetAllRentalSubscriptionPlanAsync()
        {
            return await _recontext.SubscribePlan
                .Include(p => p.PlanDurations)
                .Select(p => new SubscribeRentalPlanResponseDto
                {
                    PlanId = p.PlanId,
                    PlanName = p.PlanName,
                    PlanDescription = p.PlanDescription,
                    PlanActive = p.PlanActive,
                    PlanUsers = p.PlanUsers,        // ADD THIS
                    PlanIsAddOn = p.PlanIsAddOn,    // ADD THIS
                    Durations = p.PlanDurations
                        .Where(d => d.IsActive)
                        .Select(d => new SubscribeRentalPlanDurationDto
                        {
                            PlanDurationId = d.PlanDurationId,
                            DurationDays = d.DurationDays,
                            Price = d.Price
                        })
                        .ToList()
                })
                .ToListAsync();
        }

        public async Task<SubscribeRentalPlanResponseDto?> GetSubscriptionRentalPlanByIdAsync(int planId)
        {
            return await _recontext.SubscribePlan
                .Include(p => p.PlanDurations)
                .Where(p => p.PlanId == planId)
                .Select(p => new SubscribeRentalPlanResponseDto
                {
                    PlanId = p.PlanId,
                    PlanName = p.PlanName,
                    PlanDescription = p.PlanDescription,
                    PlanActive = p.PlanActive,
                    PlanUsers = p.PlanUsers,
                    PlanIsAddOn = p.PlanIsAddOn,
                    // ADD THIS
                    Durations = p.PlanDurations
                        .Where(d => d.IsActive)
                        .Select(d => new SubscribeRentalPlanDurationDto
                        {
                            PlanDurationId = d.PlanDurationId,
                            DurationDays = d.DurationDays,
                            Price = d.Price,
                           
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> CreateRentalSubscriptionAsync(CompanyRentalSubscriptionCreateDto dto)
        {
            using var tx = await _recontext.Database.BeginTransactionAsync();
            try
            {
                var activeSubscription = await _recontext.CompanySubscription
                    .FirstOrDefaultAsync(s =>
                        s.CompanyId == dto.CompanyId &&
                        s.Status == "ACTIVE");

                if (activeSubscription != null)
                {
                    activeSubscription.Status = "EXPIRED";
                    activeSubscription.SubscriptionEndDate = DateTime.Now;
                    await _recontext.SaveChangesAsync();
                }

      
                var plan = await _recontext.SubscribePlan
                    .FirstOrDefaultAsync(p =>
                        p.PlanId == dto.PlanId &&
                        p.PlanActive);

                if (plan == null)
                    throw new Exception("Invalid or inactive plan");

                var duration = await _recontext.SubscribePlanDurations
                    .FirstOrDefaultAsync(d =>
                        d.PlanDurationId == dto.PlanDurationId &&
                        d.PlanId == dto.PlanId &&
                        d.IsActive);

                if (duration == null)
                    throw new Exception("Invalid or inactive duration");

                var startDate = DateTime.Now;
                var endDate = startDate
                    .AddDays(duration.DurationDays)
                    .AddTicks(-1);

                var subscription = new XRS_CompanySubscription
                {
                    PlanId = plan.PlanId,
                    PlanDurationId = duration.PlanDurationId, // ✅ ADD THIS
                    CompanyId = dto.CompanyId,
                    SubscriptionStartDate = startDate,
                    SubscriptionEndDate = endDate,
                    SubscriptionDays = duration.DurationDays,
                    SubscriptionAmount = duration.Price,
                    //SubscriptionUserCount = plan.PlanUsers,  // ✅ ADD THIS
                    SubscriptionUserCount = plan.PlanUsers ,  // ✅ fix null
                    Status = "ACTIVE"
                };

                _recontext.CompanySubscription.Add(subscription);
                await _recontext.SaveChangesAsync();

                await tx.CommitAsync();
                return subscription.SubId;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        //public async Task<int> CreateRentalAddonAsync(CompanyRentalSubscriptionAddonCreateDto dto)
        //{
        //    // ✅ Validate active subscription exists
        //    var activeSub = await _recontext.CompanySubscription
        //        .FirstOrDefaultAsync(s =>
        //            s.CompanyId == dto.CompanyId &&
        //            s.SubId == dto.MainPlanId &&
        //            s.Status == "ACTIVE");

        //    if (activeSub == null)
        //        throw new Exception("No active subscription found for this company");

        //    // ✅ Validate addon plan exists
        //    var plan = await _recontext.SubscribePlan
        //        .FirstOrDefaultAsync(p =>
        //            p.PlanId == dto.PlanId &&
        //            p.PlanActive == true);

        //    if (plan == null)
        //        throw new Exception("Invalid or inactive addon plan");

        //    // ✅ Create addon using correct model
        //    var addon = new XRS_CompanySubscriptionAddon
        //    {
        //        MainPlanId = dto.MainPlanId,
        //        PlanId = dto.PlanId,
        //        CompanyId = dto.CompanyId,
        //        Amount = dto.Amount,
        //        UserCount = dto.UserCount,
        //        Status = "ACTIVE"
        //    };

        //    _recontext.CompanySubscriptionAddon.Add(addon);
        //    await _recontext.SaveChangesAsync();

        //    return addon.SubAddonId;
        //}




        public async Task<int> CreateRentalAddonAsync(CompanyRentalSubscriptionAddonCreateDto dto)
        {
            // ✅ First, verify the subscription exists and is ACTIVE
            var activeSub = await _recontext.CompanySubscription
                .FirstOrDefaultAsync(s => s.SubId == dto.MainPlanId);

            if (activeSub == null)
                throw new Exception($"Subscription with ID {dto.MainPlanId} not found");

            if (activeSub.CompanyId != dto.CompanyId)
                throw new Exception($"Subscription {dto.MainPlanId} does not belong to company {dto.CompanyId}");

            if (activeSub.Status != "ACTIVE")
                throw new Exception($"Subscription status is '{activeSub.Status}', not 'ACTIVE'");

            if (activeSub.SubscriptionEndDate < DateTime.Now)
                throw new Exception("Subscription has expired");

            // ✅ Rest of the code remains the same...
            var plan = await _recontext.SubscribePlan
                .FirstOrDefaultAsync(p => p.PlanId == dto.PlanId && p.PlanActive == true);

            if (plan == null)
                throw new Exception("Invalid or inactive addon plan");

            var existingAddon = await _recontext.CompanySubscriptionAddon
                .FirstOrDefaultAsync(a =>
                    a.MainPlanId == dto.MainPlanId &&
                    a.PlanId == dto.PlanId &&
                    a.CompanyId == dto.CompanyId &&
                    a.Status == "ACTIVE");

            if (existingAddon != null)
                throw new Exception("This addon is already active");

            var addon = new XRS_CompanySubscriptionAddon
            {
                MainPlanId = dto.MainPlanId,
                PlanId = dto.PlanId,
                CompanyId = dto.CompanyId,
                Amount = dto.Amount,
                UserCount = dto.UserCount,
                Status = "ACTIVE",
                //CreatedOn = DateTime.UtcNow
            };

            _recontext.CompanySubscriptionAddon.Add(addon);
            await _recontext.SaveChangesAsync();


            return addon.Id;  // ← Returns Id, not SubAddonId
        }
        //public async Task<SubscriptionRentalSummaryDto?> GetRentalSubscriptionSummaryAsync(int companyId)
        //{
        //    var latestSub = await _recontext.CompanySubscription
        //        .Where(s => s.CompanyId == companyId)
        //        .OrderByDescending(s => s.SubscriptionEndDate)
        //        .FirstOrDefaultAsync();

        //    if (latestSub == null) return null;

        //    var planName = await _recontext.SubscribePlan
        //        .Where(p => p.PlanId == latestSub.PlanId)
        //        .Select(p => p.PlanName)
        //        .FirstOrDefaultAsync();

        //    var addons = await _recontext.CompanySubscriptionAddon
        //        .Where(a => a.CompanyId == companyId && a.MainPlanId == latestSub.SubId && a.Status == "ACTIVE")
        //        .Select(a => new SubscriptionAddonDto
        //        {
        //            PlanId = a.PlanId,
        //            Amount = a.Amount,
        //            UserCount = a.DepCount,
        //            Status = a.Status
        //        })
        //        .ToListAsync();

        //    var history = await _recontext.CompanySubscription
        //        .Where(s => s.CompanyId == companyId)
        //        .OrderByDescending(s => s.SubscriptionEndDate)
        //        .Select(s => new SubscriptionHistoryDto
        //        {
        //            SubId = s.SubId,
        //            StartDate = s.SubscriptionStartDate,
        //            EndDate = s.SubscriptionEndDate,
        //            Amount = s.SubscriptionAmount ?? 0,
        //            Status = s.Status
        //        })
        //        .ToListAsync();

        //    return new SubscriptionRentalSummaryDto
        //    {
        //        SubId = latestSub.SubId,
        //        Status = latestSub.Status,
        //        StartDate = latestSub.SubscriptionStartDate,
        //        EndDate = latestSub.SubscriptionEndDate,
        //        Amount = latestSub.SubscriptionAmount ?? 0,
        //        UserCount = latestSub.SubscriptionUserCount ?? 0,
        //        PlanName = planName ?? string.Empty,
        //        DurationDays = latestSub.SubscriptionDays ?? 0,
        //        Addons = addons,
        //        SubscriptionHistory = history
        //    };
        //}

        public async Task<SubscriptionRentalSummaryDto?> GetRentalSubscriptionSummaryAsync(int companyId)
        {
            var latestSub = await _recontext.CompanySubscription
                .Where(s => s.CompanyId == companyId)
                .OrderByDescending(s => s.SubscriptionEndDate)
                .FirstOrDefaultAsync();

            if (latestSub == null) return null;

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

            // ✅ Fixed: use UserCount not DepCount, use SubscriptionRentalAddonDto
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
                    UserCount = a.UserCount,   // ✅ UserCount not DepCount
                    Status = a.Status
                }
            ).ToListAsync();

            // ✅ Calculate real status
            string realStatus = latestSub.Status?.Trim().ToUpper() ?? "UNKNOWN";
            var currentDate = DateTime.Now;

            if (realStatus == "ACTIVE" && latestSub.SubscriptionEndDate < currentDate)
                realStatus = "EXPIRED";
            else if (realStatus == "TRIAL" && latestSub.SubscriptionEndDate < currentDate)
                realStatus = "TRIAL_EXPIRED";

            return new SubscriptionRentalSummaryDto
            {
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
        #endregion

        #region TICKET

        public async Task<int> CreateTicketSubscribePlanAsync(SubscribeTicketPlanRequestDto request)
        {
            var plan = new TI_SubscribePlan
            {
                PlanName = request.PlanName,
                PlanDescription = request.PlanDescription,
                PlanUsers = request.PlanUsers,
                PlanIsAddOn = request.PlanIsAddOn,
                PlanCreatedBy = 0,
                PlanModifiedBy = 0,
                PlanModifiedOn = DateTime.Now,
                PlanActive = request.PlanActive
            };

            _ticontext.SubscribePlan.Add(plan);
            await _ticontext.SaveChangesAsync();

            var duration = new TI_SubscribePlanDuration
            {
                PlanId = plan.PlanId,
                DurationDays = request.PlanDurationDays,
                Price = request.PlanPrice,
                IsActive = true,
                CreatedOn = DateTime.UtcNow  // ← add this
            };

            _ticontext.SubscribePlanDuration.Add(duration);
            await _ticontext.SaveChangesAsync();

            return plan.PlanId;
        }
        
        public async Task<bool> CreateTicketSubscribeUpdateAsync( int planId, SubscribeTicketPlanRequestDto request)
        {
            var plan = await _ticontext.SubscribePlan
                .Include(p => p.PlanDurations)
                .FirstOrDefaultAsync(p => p.PlanId == planId);

            if (plan == null) return false;

            plan.PlanName = request.PlanName;
            plan.PlanDescription = request.PlanDescription;
            plan.PlanUsers = request.PlanUsers;
            plan.PlanIsAddOn = request.PlanIsAddOn;
            plan.PlanActive = request.PlanActive;
            plan.PlanModifiedBy = 0;
            plan.PlanModifiedOn = DateTime.Now;

            _ticontext.SubscribePlanDuration.RemoveRange(plan.PlanDurations);

            plan.PlanDurations.Add(new TI_SubscribePlanDuration
            {
                DurationDays = request.PlanDurationDays,
                Price = request.PlanPrice,
                IsActive = true
            });

            await _ticontext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<SubscribeTicketPlanResponseDto>>GetAllTicketSubscriptionPlanAsync()
        {
            return await _ticontext.SubscribePlan
                .Where(p => p.PlanActive)
                .Select(p => new SubscribeTicketPlanResponseDto
                {
                    PlanId = p.PlanId,
                    PlanName = p.PlanName,
                    PlanDescription = p.PlanDescription,
                    PlanIsAddOn = p.PlanIsAddOn,
                    PlanUsers = p.PlanUsers,
                    PlanActive = p.PlanActive,

                    Durations = p.PlanDurations
                        .Where(d => d.IsActive)
                        .Select(d => new TicketPlanDurationDto
                        {
                            PlanDurationId = d.PlanDurationId,
                            DurationDays = d.DurationDays,
                            Price = d.Price
                        })
                        .ToList()
                })
                .ToListAsync();
        }

        public async Task<SubscribeTicketPlanResponseDto?> GetSubscriptionTicketPlanByIdAsync(int planId)
        {
            return await _ticontext.SubscribePlan
                .Where(p => p.PlanId == planId)
                .Select(p => new SubscribeTicketPlanResponseDto
                {
                    PlanId = p.PlanId,
                    PlanName = p.PlanName,
                    PlanDescription = p.PlanDescription,
                    PlanIsAddOn = p.PlanIsAddOn,
                    PlanUsers = p.PlanUsers,
                    PlanActive = p.PlanActive,
                    Durations = p.PlanDurations
                        .Where(d => d.IsActive)
                        .Select(d => new TicketPlanDurationDto
                        {
                            PlanDurationId = d.PlanDurationId,
                            DurationDays = d.DurationDays,
                            Price = d.Price
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> CreateTicketSubscriptionAsync(CompanyTicketSubscriptionCreateDto dto)
        {
            using var transaction = await _ticontext.Database.BeginTransactionAsync();

            try
            {
                var activeSubscription = await _ticontext.CompanySubscription
                    .FirstOrDefaultAsync(s =>
                        s.CompanyId == dto.CompanyId &&
                        s.Status == "ACTIVE");

                if (activeSubscription != null)
                {
                    activeSubscription.Status = "EXPIRED";
                    activeSubscription.SubscriptionEndDate = DateTime.Now;
                    await _ticontext.SaveChangesAsync();
                }

                var duration = await _ticontext.SubscribePlanDuration
                    .Include(d => d.Plan)
                    .FirstOrDefaultAsync(d =>
                        d.PlanDurationId == dto.PlanDurationId &&
                        d.IsActive);

                if (duration == null)
                    throw new Exception("Invalid plan duration");

                var startDate = DateTime.Now;
                var endDate = startDate
                    .AddDays(duration.DurationDays)
                    .AddTicks(-1);

                var subscription = new TI_CompanySubscription
                {
                    PlanId = duration.PlanId,
                    CompanyId = dto.CompanyId,
                    SubscriptionStartDate = startDate,
                    SubscriptionEndDate = endDate,
                    SubscriptionDays = duration.DurationDays,
                    SubscriptionAmount = duration.Price,
                    SubscriptionUserCount = duration.Plan.PlanUsers,
                    Status = "ACTIVE"
                };

                _ticontext.CompanySubscription.Add(subscription);
                await _ticontext.SaveChangesAsync();
   
                if (dto.Addons?.Any() == true)
                {
                    var addons = dto.Addons.Select(a => new TI_CompanySubscriptionAddon
                    {
                        MainPlanId = subscription.SubId,
                        PlanId = a.PlanId,
                        CompanyId = dto.CompanyId,
                        Amount = a.Amount,
                        UserCount = a.UserCount,
                        Status = "ACTIVE"
                    });

                    _ticontext.CompanySubscriptionAddon.AddRange(addons);
                    await _ticontext.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return subscription.SubId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        #endregion

    }

}
