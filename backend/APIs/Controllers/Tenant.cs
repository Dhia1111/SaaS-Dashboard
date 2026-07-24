using APIs.ConfigClasses;
using APIs.Responses;
using Business;
using Connection.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MimeKit.Tnef;

namespace APIs.Controllers
{
    [ApiController]
    [Route("api/tenat")]
    public class TenantController : ControllerBase
    {
        private readonly ITenantService _tenantService;
        private readonly ITenantIdProvider _tenantIdProvider;
        private readonly IClientSubscriptionService _clientSubscriptionService;

        public TenantController(
            ITenantService tenantervice,
            ITenantIdProvider tenantIdProvider,
            IClientSubscriptionService clientSubscriptionService)
        {
            _tenantService = tenantervice;
            _tenantIdProvider = tenantIdProvider;
            _clientSubscriptionService = clientSubscriptionService;
        }

       [Authorize]
        [RequiersdClaim("ReadForTenantInfo",SharedDto_Enum.enPlaformRoles.User)]
        [HttpGet("tenant")]
        public async Task<ActionResult<ApiResult<DtoTenant>>> GetById ([FromQuery] int TenantId)
        {

                      var tenant = await _tenantService.GetByIdAsync(TenantId);
            tenant.PasswordHash = null;
            return Ok(ApiResult<DtoTenant>.Ok(tenant, "Tenant fetched successfully"));
        }

        [Authorize]
        [RequiersdClaim("ReadForClientSubscriptionInfo", SharedDto_Enum.enPlaformRoles.User)]
        [HttpGet("clients-subscriptions")]
        public async Task<ActionResult<ApiResult<IReadOnlyList<DtoClientSubscription>>>>ClientSubscriptionList()
        {

            var res= await _clientSubscriptionService.GetAllAsync();

            return Ok(ApiResult<IReadOnlyList<DtoClientSubscription>>.Ok(res));


        }
 
   
     

    }
}