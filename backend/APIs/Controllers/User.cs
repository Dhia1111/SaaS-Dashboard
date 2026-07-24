using APIs.ConfigClasses;
using APIs.Responses;
using Business;
using Connection.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedDto_Enum;
using System.Threading.Tasks;

namespace APIs.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITenantIdProvider _tenantIdProvider;

        public UserController(
            IUserService userService,
            ITenantIdProvider tenantIdProvider)
        {
            _userService = userService;
            _tenantIdProvider = tenantIdProvider;
        }

        // GET: api/user
        [Authorize]
        [RequiersdClaim("ReadForUsers", SharedDto_Enum.enPlaformRoles.User)]
        [HttpGet]
        public async Task<ActionResult<ApiResult<IReadOnlyList<DtoUser>>>> GetAll()
        {
            var users = await _userService.GetAllAsync();

            return Ok(ApiResult<IReadOnlyList<DtoUser>>
                .Ok(users, "Users fetched successfully"));
        }

        // GET: api/user/{id}

        [Authorize]
        [RequiersdClaim("ReadForUsers", SharedDto_Enum.enPlaformRoles.User)]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResult<DtoUser>>> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);

            return Ok(ApiResult<DtoUser>
                .Ok(user, "User fetched successfully"));
        }

        [Authorize]
        [RequiersdClaim("WriteForUsers", SharedDto_Enum.enPlaformRoles.User)]
        // PUT: api/user/{id}
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResult<bool>>> Update(int id, [FromBody] DtoUser dto)
        {
            if (id != dto.Id)
                throw new ArgumentException("Route id mismatch");

            var result = await _userService.UpdateAsync(dto);

            return Ok(ApiResult<bool>
                .Ok(result, "User updated successfully"));
        }

        // DELETE: api/user/{id}
        [Authorize]
        [RequiersdClaim("WriteForUsers", SharedDto_Enum.enPlaformRoles.User)]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResult<bool>>> Delete(int id)
        {
            var result = await _userService.DeleteAsync(id);

            return Ok(ApiResult<bool>
                .Ok(result, "User deleted successfully"));
        }


        // GET: api/user/roles
        [Authorize]
        [RequiersdClaim("ReadForUsers", SharedDto_Enum.enPlaformRoles.User)]
        [HttpGet("roles")]
        public ActionResult GetRoles()
        {
            var roles = Enum.GetValues(typeof(enRoles))
                .Cast<enRoles>()
                .Select(x => new KeyValuePair<int, string>((int)x, x.ToString().Replace("__"," ")))
                .ToList();

            return Ok(ApiResult<object>
                .Ok(roles, "Roles fetched successfully"));
        }

        // GET: api/user/authorization-options
        [Authorize]
        [RequiersdClaim("ReadForUsers", SharedDto_Enum.enPlaformRoles.User)]
        [HttpGet("authorization-options")]
        public async Task<ActionResult<List<KeyValuePair<long,string>>>> GetAuthorizationOptions()
        {
            // you recive the platform 

            var list =await _userService.GetTenantPermissionForUsers(); ;
            return Ok(ApiResult<List<KeyValuePair<long, string>>>
                .Ok(list, "Authorization options fetched successfully"));
        }
    }
}