// Business/clsUserSubscriptionService.cs
using Connection.models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business
{
    public interface IUserSubscriptionService : IGenericService<DtoUserSubscription>
    {
    }

    public class clsUserSubscriptionService : GenericService<DtoUserSubscription, UserSubscription>, IUserSubscriptionService
    {
        private readonly IUserSubscriptionRepo _repo;
        private readonly ILogger<clsUserSubscriptionService> _logger;

        public clsUserSubscriptionService(IUserSubscriptionRepo repo, ILogger<clsUserSubscriptionService> logger)
            : base(repo, logger)
        {
            _repo = repo;
            _logger = logger;
        }

        protected override DtoUserSubscription ToDto(UserSubscription entity)
        {
            return new DtoUserSubscription
            {
                Id = entity.Id,
                TenantId = entity.TenantId,
                UserId = entity.Id,
                SubscriptionTypeId = entity.SubscriptionTypeId,
                Status = entity.Status,
                StripeSubscriptionId = entity.StripeSubscriptionId,
                StartDate = entity.StartedAt.ToString("O"),
                EndDate = entity.EndsAt?.ToString("O")
            };
        }

        protected override UserSubscription FromDto(DtoUserSubscription dto)
        {
            return new UserSubscription
            {
                Id = dto.Id,
                TenantId = dto.TenantId,
                UserId = dto.UserId,
                SubscriptionTypeId = dto.SubscriptionTypeId,
                Status = dto.Status,
                StripeSubscriptionId = dto.StripeSubscriptionId,
                StartedAt = DateTime.TryParse(dto.StartDate, out var start) ? start : throw new Exception("Invalid StartedAt string"),
                EndsAt = string.IsNullOrWhiteSpace(dto.EndDate) ? null : DateTime.TryParse(dto.EndDate, out var end) ? end : throw new Exception("Invalid EndsAt string")
            };
        }
    }
}
