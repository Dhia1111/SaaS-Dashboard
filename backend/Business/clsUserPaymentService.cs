// Business/clsUserPaymentService.cs
using Connection.models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Business
{
    public interface IUserPaymentService : IGenericService<DtoUserPayment>
    {
        Task<UserPayment?> GetByStripePaymentIntentIdAsync(string stripePaymentIntentId);
        Task<IReadOnlyList<DtoUserPayment>> GetByUserReferenceAsync(int tenantId, string userReferenceId);
    }

    public class clsUserPaymentService : GenericService<DtoUserPayment, UserPayment>, IUserPaymentService
    {
        private readonly IUserPaymentRepo _repoTyped;
        private readonly ILogger<clsUserPaymentService> _typedLogger;

        public clsUserPaymentService(
            IUserPaymentRepo repo,
            ILogger<clsUserPaymentService> logger)
            : base(repo , logger)
        {
            _repoTyped = repo;
            _typedLogger = logger;
        }

        protected override DtoUserPayment ToDto(UserPayment entity)
        {
            return new DtoUserPayment
            {
                Id = entity.Id,
                TenantId = entity.TenantId,
                DataKey = entity.DataKey,
                UserReferenceId = entity.UserReferenceId,
                Amount = entity.Amount,
                Currency = entity.Currency,
                Status = entity.Status,
                StripePaymentIntentId = entity.StripePaymentIntentId,
                PaidAt = entity.PaidAt.ToString("O") // ISO 8601
            };
        }

        protected override UserPayment FromDto(DtoUserPayment dto)
        {
            // Parse PaidAt string → DateTime explicit with try/catch as you requested
            DateTime paidAt;
            try
            {
                if (!DateTime.TryParse(dto.PaidAt, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out paidAt))
                {
                    // Throw to signal invalid input format
                    _typedLogger.LogError("Invalid PaidAt format: {PaidAt}", dto.PaidAt);
                    throw new FormatException($"Invalid PaidAt value: {dto.PaidAt}");
                }
            }
            catch (Exception ex)
            {
                _typedLogger.LogError(ex, "Error parsing PaidAt from DtoUserPayment");
                throw;
            }

            return new UserPayment
            {
                Id = dto.Id,
                TenantId = dto.TenantId,
                DataKey = dto.DataKey,
                UserReferenceId = dto.UserReferenceId,
                Amount = dto.Amount,
                Currency = dto.Currency,
                Status = dto.Status,
                StripePaymentIntentId = dto.StripePaymentIntentId,
                PaidAt = paidAt
            };
        }

        public async Task<UserPayment?> GetByStripePaymentIntentIdAsync(string stripePaymentIntentId)
        {
            var entity = await _repoTyped.GetByStripePaymentIntentIdAsync(stripePaymentIntentId);
            return entity;
        }

        public async Task<IReadOnlyList<DtoUserPayment>> GetByUserReferenceAsync(int tenantId, string userReferenceId)
        {
            var list = await _repoTyped.GetByUserReferenceAsync(tenantId, userReferenceId);
            return list.Select(ToDto).ToList();
        }


    }
}
