using APIs.ConfigClasses;
using APIs.Responses;
using Business;
using Business.Exceptions;
using Connection.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIs.Controllers
{
    [Route("api/tenant/subscription")]
    [ApiController]
    public class Subscriptions:ControllerBase
    {
        private readonly ITenantPlanServices _tenantPlanService;
        private readonly ITenantIdProvider _tenantIdProvider;

        public Subscriptions(ITenantPlanServices planServices, ITenantIdProvider tenantIdProvider)
        {

            _tenantPlanService = planServices;
            _tenantIdProvider = tenantIdProvider;
        }

        [Authorize]
        [RequiersdClaim("ReadForPlan", SharedDto_Enum.enPlaformRoles.User)]
        [HttpGet("GetTenantPlans")]
        public async Task<ActionResult> GetTenantPlans()
        {
            var tenantPlans = await _tenantPlanService.GetAllAsync();   

            return Ok(ApiResult<IReadOnlyList<DtoTenantPlan>>.Ok(tenantPlans ));
        }
     
        [Authorize]
        [RequiersdClaim("WriteForPlan", SharedDto_Enum.enPlaformRoles.User)]
        [HttpPost("AddNewSubscription")]
        public async Task<ActionResult> AddSubscription([FromBody] DtoAddNewTenantPlan request)
        {
            //check if the permission key already exists for the tenant

            bool exists = await _tenantPlanService.IsTenantPlanNameExist(request.TenantPlan.Name);
            if (exists)
            {
                throw new ResourceAlreadyExistsException("Tenant", request.TenantPlan.Name);
            }
            int newId = await _tenantPlanService.AddNewTenantPlanWithDependancies(request);
            return Ok(ApiResult<int>.Ok(newId));
        }


        [Authorize]
        [RequiersdClaim("WriteForPlan", SharedDto_Enum.enPlaformRoles.User)]
        [HttpPut("UpdateTenantSubscription")]
        public async Task<ActionResult> UpdateSubscription([FromBody] DtoAddNewTenantPlan request)
        {
            bool result = await _tenantPlanService.UpdateTenantPlanWithDependancies(request);
            return Ok(ApiResult<bool>.Ok(result));
        }


        [Authorize]
        [RequiersdClaim("WriteForPlan", SharedDto_Enum.enPlaformRoles.User)]
        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            bool res = await _tenantPlanService.DeleteAsync(id);
            return Ok(ApiResult<bool>.Ok(res));
        }

        [Authorize]
        [RequiersdClaim("ReadForPlan", SharedDto_Enum.enPlaformRoles.User)]
        [HttpGet("GetSubscriptionList")]
        public async Task<ActionResult<IEnumerable<DtoTenantPlan>>> GetSubscriptionListAsync()
        {
            var res = await _tenantPlanService.GetAllWithDependenciesAsync();

            return Ok(ApiResult<IEnumerable<DtoTenantPlan>>.Ok(res));
        }


    }

   }

