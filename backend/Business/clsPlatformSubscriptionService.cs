// Business/clsPlatformSubscriptionService.cs
using Connection.models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Business
{
    public interface IPlatformSubscriptionService : IGenericService<DtoPlatformSubscription>
    {
        Task<DtoPlatformSubscription?> GetByStripeSubscriptionIdAsync(string stripeSubscriptionId);
    }

    public class clsPlatformSubscriptionService : GenericService<DtoPlatformSubscription, PlatformSubscription>, IPlatformSubscriptionService
    {
        private readonly IPlatformSubscriptionRepo _typedRepo;
        private readonly ILogger<clsPlatformSubscriptionService> _typedLogger;

        public clsPlatformSubscriptionService(IPlatformSubscriptionRepo repo, ILogger<clsPlatformSubscriptionService> logger)
            : base(repo , logger)
        {
            _typedRepo = repo;
            _typedLogger = logger;
        }

        protected override DtoPlatformSubscription ToDto(PlatformSubscription entity)
        {
            return new DtoPlatformSubscription
            {
                Id = entity.Id,
                TenantId = entity.TenantId,
                PlatformPlanId = entity.PlatformPlanId,
                Status = entity.Status,
                StripeSubscriptionId = entity.StripeSubscriptionId,
                StartedAt = entity.StartedAt.ToString("O"),
                EndsAt = entity.EndsAt?.ToString("O") ?? string.Empty,
                Tenant = entity.Tenant != null ? new DtoTenant { Id = entity.Tenant.Id, CompanyName = entity.Tenant.CompanyName } : null!,
                PlatformPlan = entity.PlatformPlan != null ? new DtoPlatformPlan { Id = entity.PlatformPlan.Id, Name = entity.PlatformPlan.Name } : null!
            };
        }

        protected override PlatformSubscription FromDto(DtoPlatformSubscription dto)
        {
            DateTime startedAt;
            DateTime? endsAt = null;

            try
            {
                if (!DateTime.TryParse(dto.StartedAt, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out startedAt))
                {
                    _typedLogger.LogError("Invalid StartedAt format: {StartedAt}", dto.StartedAt);
                    throw new FormatException($"Invalid StartedAt: {dto.StartedAt}");
                }

                if (!string.IsNullOrWhiteSpace(dto.EndsAt))
                {
                    if (!DateTime.TryParse(dto.EndsAt, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var e))
                    {
                        _typedLogger.LogError("Invalid EndsAt format: {EndsAt}", dto.EndsAt);
                        throw new FormatException($"Invalid EndsAt: {dto.EndsAt}");
                    }
                    endsAt = e;
                }
            }
            catch (Exception ex)
            {
                _typedLogger.LogError(ex, "Error parsing dates in DtoPlatformSubscription");
                throw;
            }

            return new PlatformSubscription
            {
                Id = dto.Id,
                TenantId = dto.TenantId,
                PlatformPlanId = dto.PlatformPlanId,
                Status = dto.Status,
                StripeSubscriptionId = dto.StripeSubscriptionId,
                StartedAt = startedAt,
                EndsAt = endsAt
            };
        }

        public async Task<DtoPlatformSubscription?> GetByStripeSubscriptionIdAsync(string stripeSubscriptionId)
        {
            var e = await _typedRepo.GetByStripeSubscriptionIdAsync(stripeSubscriptionId);
            return e != null ? ToDto(e) : null;
        }
    }
}
