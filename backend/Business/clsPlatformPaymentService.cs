// Business/clsPlatformPaymentService.cs
using Connection.models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business
{
    public interface IPlatformPaymentService : IGenericService<DtoPlatformPayment>
    {
    }

    public class clsPlatformPaymentService : GenericService<DtoPlatformPayment, PlatformPayment>, IPlatformPaymentService
    {
        private readonly IPlatformPaymentRepo _repo;
        private readonly ILogger<clsPlatformPaymentService> _logger;

        public clsPlatformPaymentService(IPlatformPaymentRepo repo, ILogger<clsPlatformPaymentService> logger)
            : base(repo, logger)
        {
            _repo = repo;
            _logger = logger;
        }

        protected override DtoPlatformPayment ToDto(PlatformPayment entity)
        {
            return new DtoPlatformPayment
            {
                Id = entity.Id,
                TenantId = entity.TenantId,
                PlatformPlanId= entity.PlatformPlanId,
                Amount = entity.Amount,
                Status = entity.Status,
                StripePaymentIntentId = entity.StripePaymentIntentId,
                PaymentDate = entity.PaidAt.ToString("O")
            };
        }

        protected override PlatformPayment FromDto(DtoPlatformPayment dto)
        {
            return new PlatformPayment
            {
                Id = dto.Id,
                TenantId = dto.TenantId,
                PlatformPlanId = dto.PlatformPlanId,
                Amount = dto.Amount,
                Status = dto.Status,
                StripePaymentIntentId = dto.StripePaymentIntentId,
                PaidAt = DateTime.TryParse(dto.PaymentDate, out var paid) ? paid : throw new Exception("Invalid PaidAt string")
            };
        }
    }
}
