// Business/clsTenantPlanService.cs
using Connection.models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business
{
    public interface ITenantPlanService : IGenericService<DtoTenantPlan>
    {
    }

    public class clsTenantPlanService : GenericService<DtoTenantPlan, TenantPlan>, ITenantPlanService
    {
        

        public clsTenantPlanService(ITenantPlanRepo repo, ILogger<clsTenantPlanService> logger)
            : base(repo, logger)
        {
           
        }

        protected override DtoTenantPlan ToDto(TenantPlan entity)
        {
            return new DtoTenantPlan
            {
                Id = entity.Id,
                DataKey = entity.DataKey,
                TenantId = entity.TenantId,
                Name = entity.Name,
                Description = entity.Description,
                Price = entity.Price,
                BillingInterval = entity.BillingInterval,
                StripePriceId = entity.StripePriceId,
                IsActive = entity.IsActive
            };
        }

        protected override TenantPlan FromDto(DtoTenantPlan dto)
        {
            return new TenantPlan
            {
                Id = dto.Id,
                DataKey = dto.DataKey,
                TenantId = dto.TenantId,
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                BillingInterval = dto.BillingInterval,
                StripePriceId = dto.StripePriceId,
                IsActive = dto.IsActive
            };
        }
    }
}
