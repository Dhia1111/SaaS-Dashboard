// APIs/Controllers/UsersController.cs

using APIs.Responses;
using Business;
using Connection.models;
using Microsoft.AspNetCore.Mvc;

namespace APIs.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            IUserService userService,
            ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        // GET: api/user/list
        [HttpGet("list")]
        public async Task<ActionResult<ApiResult<IReadOnlyList<DtoUser>>>> GetAll()
        {
            var users = await _userService.GetAllAsync();

            return Ok(
                ApiResult<IReadOnlyList<DtoUser>>
                .Ok(users, "Users fetched successfully")
            );
        }

        // GET: api/users/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResult<DtoUser>>> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);

            return Ok(
                ApiResult<DtoUser>
                .Ok(user, "User fetched successfully")
            );
        }

        // POST: api/users
        [HttpPost]
        public async Task<ActionResult<ApiResult<int>>> Add([FromBody] DtoUser dto)
        {
            if (!ModelState.IsValid)
                throw new ArgumentException("Invalid payload.");

            var newId = await _userService.AddAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = newId },
                ApiResult<int>.Ok(newId, "User created successfully")
            );
        }

        // PUT: api/users/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResult<bool>>> Update(
            int id,
            [FromBody] DtoUser dto)
        {
            if (id != dto.Id)
                throw new ArgumentException("Route id does not match body id.");

            var updated = await _userService.UpdateAsync(dto);

            return Ok(
                ApiResult<bool>
                .Ok(updated, "User updated successfully")
            );
        }

        // DELETE: api/users/5
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResult<bool>>> Delete(int id)
        {
            var deleted = await _userService.DeleteAsync(id);

            return Ok(
                ApiResult<bool>
                .Ok(deleted, "User deleted successfully")
            );
        }
    }
}