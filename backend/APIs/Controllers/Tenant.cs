using APIs.ConfigClasses;
using APIs.Responses;
using Business;
using Connection.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MimeKit.Tnef;
using System.Threading.Tasks;

namespace APIs.Controllers
{
    [ApiController]
    [Route("api/tenat")]
    public class TenantController : ControllerBase
    {
        private readonly ITenantService _tenantService;
         private readonly IClientSubscriptionService _clientSubscriptionService;
        private readonly IDomainService _domainService;
 
        public TenantController(
            ITenantService tenantervice,
            ITenantIdProvider tenantIdProvider,
            IClientSubscriptionService clientSubscriptionService,
            IDomainService domainService,
            IDomainsLoader domainsLoader)
        {
            _tenantService = tenantervice;
             _clientSubscriptionService = clientSubscriptionService;
            _domainService = domainService;
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

        [Authorize]
        [RequiersdClaim("manage-account", SharedDto_Enum.enPlaformRoles.User)]
        [HttpPost("create-subdomain")]
        public async Task<ActionResult<ApiResult<int>>> Add([FromBody] DtoDomain request)
        {
            
            var res=await  _domainService.AddAsync(request);

            return Ok(ApiResult<int>.Ok(res));

 
        }

        [Authorize]
        [RequiersdClaim("manage-account", SharedDto_Enum.enPlaformRoles.User)]
        
        [HttpPut("update-subdomain")]
        public async Task<ActionResult<ApiResult<bool>>> Update([FromBody] DtoDomain request)
        {

            var res = await _domainService.UpdateAsync(request);
            return Ok(ApiResult<bool>.Ok(res));


        }
       
        [Authorize]
        [RequiersdClaim("manage-account", SharedDto_Enum.enPlaformRoles.User)]

        [HttpDelete("delete-subdomain")]
        public async Task<ActionResult<ApiResult<bool>>> Delete([FromBody] int request)
        {

            var res = await _domainService.DeleteAsync(request);
            return Ok(ApiResult<bool>.Ok(res));


        }


    }
}