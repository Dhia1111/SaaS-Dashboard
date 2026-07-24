using APIs.ConfigClasses;
using APIs.Responses;
using Business;
using Business.Config;
using Connection.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SharedDto_Enum;
using System.Threading.Tasks;

namespace APIs.Controllers
{
    [Authorize]
    [Route("api/employee")]
    [ApiController]

    public class EmployeesManagment:ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly ITenantIdProvider _tenantIdProvider;
        private readonly PlatformInfo _platformInfo;
        private readonly ITenantPermissionServices _tenantPermissionServices;

        public EmployeesManagment(
            IEmployeeService userService,
            ITenantIdProvider tenantIdProvider,
            IOptions<PlatformInfo>Platforminformations,
            ITenantPermissionServices permissionServices)
        {
            _employeeService = userService;
            _tenantIdProvider = tenantIdProvider;
            _platformInfo = Platforminformations.Value;
            _tenantPermissionServices = permissionServices;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResult<IReadOnlyList<DtoEmployee>>>> GetAll()
        {
            var users = await _employeeService.GetAllAsync();

            return Ok(ApiResult<IReadOnlyList<DtoEmployee>>
                .Ok(users, "Users fetched successfully"));
        }

        // GET: api/user/{id}
        [RequiersdClaim("ReadForEmployees",enPlaformRoles.Employee)]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResult<DtoEmployee>>> GetById(int id)
        {
            var user = await _employeeService.GetByIdAsync(id);

            return Ok(ApiResult<DtoEmployee>
                .Ok(user, "User fetched successfully"));
        }

        [RequiersdClaim("WriteForEmployees", enPlaformRoles.Employee)]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResult<bool>>> Update(int id, [FromBody] DtoEmployee dto)
        {
            if (id != dto.Id)
                throw new ArgumentException("Route id mismatch");

            var result = await _employeeService.UpdateAsync(dto);

            return Ok(ApiResult<bool>
                .Ok(result, "User updated successfully"));
        }

        
        [RequiersdClaim("WriteForEmployees", enPlaformRoles.Employee)]

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResult<bool>>> Delete(int id)
        {
            var result = await _employeeService.DeleteAsync(id);

            return Ok(ApiResult<bool>
                .Ok(result, "User deleted successfully"));
        }

       
        [RequiersdClaim("WriteForEmployees", enPlaformRoles.Employee)]

        [HttpPost("add")]
        public async Task<ActionResult<ApiResult<int>>> Add([FromBody] DtoEmployee platformUser)
        {

            
            var  res=await _employeeService.AddAsync(platformUser);
            return Ok(ApiResult<int>.Ok(res));
        }

        [RequiersdClaim("ReadForEmployees", enPlaformRoles.Employee)]

        [HttpGet("roles")]
        public ActionResult GetRoles()
        {
            var roles = Enum.GetValues(typeof(enPlaformRoles))
                .Cast<enPlaformRoles>()
                .Select(x => new KeyValuePair<int, string>((int)x, x.ToString())).Where(x=>x.Key==(int)enPlaformRoles.Employee)
                .ToList();

            return Ok(ApiResult<object>
                .Ok(roles, "Roles fetched successfully"));
        }


        [RequiersdClaim("ReadForEmployees", enPlaformRoles.Employee)]

        [HttpGet("authorization-options")]
        public async Task<ActionResult> GetAuthorizationOptions()
        {
            List<DtoTenantPermission> list = await _tenantPermissionServices.GetAllByTenantNameWithFilterIgnoreAsync(_platformInfo.TenantName);

            return Ok(ApiResult<List<KeyValuePair<long,string>>>
                .Ok(list.Select(e=> new KeyValuePair<long,string>(e.BitValue,e.PermissionKey)).ToList(), "Authorization options fetched successfully"));
        }


    }
}
