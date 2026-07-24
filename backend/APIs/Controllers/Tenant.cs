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

        public TenantController(
            ITenantService tenantervice,
            ITenantIdProvider tenantIdProvider)
        {
            _tenantService = tenantervice;
            _tenantIdProvider = tenantIdProvider;
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
   
     

    }
}