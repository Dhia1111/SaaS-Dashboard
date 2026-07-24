using APIs.ConfigClasses;
using APIs.Responses;
using Business;
using Business.Exceptions;
using Connection.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security;
namespace APIs.Controllers
{

    [Authorize]
[Route("api/tenant/permission")]
    [ApiController]

    public class Permissions : ControllerBase
    {
        private readonly ITenantPermissionServices _permissionsService;
        private readonly ITenantIdProvider _tenantIdProvider;

        public Permissions(ITenantPermissionServices permissionsService,ITenantIdProvider tenantIdProvider)
        {
            _permissionsService = permissionsService;
            _tenantIdProvider = tenantIdProvider;
        }

        [RequiersdClaim("ReadForSubscription", SharedDto_Enum.enPlaformRoles.User)]
        [HttpGet("GetPermissions")]
            public async Task<ActionResult> GetPermissions()
            {
                var permissions = await _permissionsService.GetAllAsync();

    
                return Ok(ApiResult<IReadOnlyList<DtoTenantPermission>>.Ok(permissions));
        }

        [RequiersdClaim("WriteForSubscription", SharedDto_Enum.enPlaformRoles.User)]
        [HttpPost("AddPermission")]
        public async Task<ActionResult> AddPermission([FromBody] DtoTenantPermission request)
        {
            //check if the permission key already exists for the tenant

            request.TenantId = _tenantIdProvider.TenantId;

            bool exists = await _permissionsService.IsPermissionKeyExist(request.PermissionKey);
            if (exists)
            {
                throw new ResourceAlreadyExistsException("Tenant", request.PermissionKey);
            }
            int newId = await _permissionsService.AddAsync(request);
            return Ok(ApiResult<int>.Ok(newId));
        }

        [RequiersdClaim("WriteForSubscription", SharedDto_Enum.enPlaformRoles.User)]
        [HttpPut("UpdatePermission")]
         public async Task<ActionResult> UpdatePermission([FromBody] DtoTenantPermission request)
        {   bool result = await _permissionsService.UpdateAsync(request);
            return Ok(ApiResult<bool>.Ok(result));
        }

        [RequiersdClaim("WriteForSubscription", SharedDto_Enum.enPlaformRoles.User)]
        [HttpDelete("DeletePermission/{id}")]
        public async Task<ActionResult> DeletePermission(int id)
        {
           bool res=  await _permissionsService.DeleteAsync(id);
            return Ok(ApiResult<bool>.Ok(res));
        }

        }
}