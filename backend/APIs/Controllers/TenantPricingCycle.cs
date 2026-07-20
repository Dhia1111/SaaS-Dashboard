using APIs.Responses;
using Business;
using Business.Exceptions;
using Connection.models;
using Connection.models.Entites;
using Microsoft.AspNetCore.Mvc;
using System.Security;
namespace APIs.Controllers
{

[Route("api/tenant/pricingcycle")]
    [ApiController]

    public class PricingCycle : ControllerBase
    {
        private readonly ITenantPricingCycleServices _pricingCycleService;
        private readonly ITenantIdProvider _tenantIdProvider;

        public PricingCycle(ITenantPricingCycleServices pricingCycleService,    ITenantIdProvider tenantIdProvider)
        {
            _pricingCycleService = pricingCycleService;
            _tenantIdProvider = tenantIdProvider;
        }

        [HttpGet("GetPricingCycles")]
            public async Task<ActionResult> GetPricingCycles()
            {
                var pricingCycles = await _pricingCycleService.GetAllAsync();

    
                return Ok(ApiResult<IReadOnlyList<DtoTenantPricingCycle>>.Ok(pricingCycles));
        }
       
        [HttpPost("AddPricingCycle")]
        public async Task<ActionResult> AddPricingCycle([FromBody] DtoTenantPricingCycle   request)
        {
            //check if the permission key already exists for the tenant

            request.TenantId = _tenantIdProvider.TenantId;
            bool isUnique = await _pricingCycleService.IsUnique(request.CycleName);
            if (!isUnique)
            {
                throw new ResourceAlreadyExistsException(nameof(ITenantPricingCycleServices), request.CycleName);
            }
            int newId = await _pricingCycleService.AddAsync(request);
            return Ok(ApiResult<int>.Ok(newId));
        }
         [HttpPut("UpdatePricingCycle")]
         public async Task<ActionResult> UpdatePricingCycle([FromBody] DtoTenantPricingCycle request)
        {   bool result = await _pricingCycleService  .UpdateAsync(request);
            return Ok(ApiResult<bool>.Ok(result));
        }

        [HttpDelete("DeletePricingCycle/{id}")]
        public async Task<ActionResult> DeletePricingCycle(int id)
        {
           bool res=  await _pricingCycleService.DeleteAsync(id);
            return Ok(ApiResult<bool>.Ok(res));
        }

        }
}