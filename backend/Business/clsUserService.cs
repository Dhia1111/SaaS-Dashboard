using Business;
using Connection.models;
using Microsoft.Extensions.Logging;

public interface IUserService
{
    Task<IReadOnlyList<DtoUser>> GetAllAsync(int dataKey);
    Task<DtoUser> GetByIdAsync(int id);
    Task<DtoUser?> GetByEmailAsync(string email);
    Task<bool> AddAsync(User user);
    Task<bool> UpdateAsync(User user);
    Task<bool> DeleteAsync(int id);
}

public class clsUserService : IUserService
{
    private readonly IUserRepo _repo;
    private readonly clsPersonService _personService;
    private readonly ILogger<clsUserService> _logger;

    public clsUserService(IUserRepo repo, clsPersonService personService, ILogger<clsUserService> logger)
    {
        _repo = repo;
        _personService = personService;
        _logger = logger;
    }


     
     
    // ======================
    // READ OPERATIONS
    // ======================
    public async Task<IReadOnlyList<DtoUser>> GetAllAsync(int dataKey)
    {
        var users = await _repo.GetAllAsync(dataKey);
        return users.Select(ToDto).ToList();
    }

    public async Task<DtoUser> GetByIdAsync(int id)
    {
        var user = await _repo.GetByIdAsync(id);
        if (user == null)
            throw new KeyNotFoundException($"User with Id {id} not found.");

        return ToDto(user);
    }

    public async Task<DtoUser?> GetByEmailAsync(string email)
    {
        var user = await _repo.GetByEmailAsync(email);
        return user != null ? ToDto(user) : null;
    }

    // ======================
    // WRITE OPERATIONS
    // ======================

    public async Task<bool> AddAsync(User user)
    {
        // Ensure Person exists
        var person = await _personService.GetByIdAsync(user.PersonID);
        if (person == null)
        {
            _logger.LogWarning("Cannot create User. Person Id {PersonId} not found.", user.PersonID);
            return false;
        }

        var result = await _repo.AddAsync(user);

        if (result)
            _logger.LogInformation("User added successfully with Id {UserId}", user.Id);
        else
            _logger.LogWarning("Failed to add User for PersonId {PersonId}", user.PersonID);

        return result;
    }

    public async Task<bool> UpdateAsync(User user)
    {
        var existing = await _repo.GetByIdAsync(user.Id);
        if (existing == null)
        {
            _logger.LogWarning("User with Id {UserId} not found for update", user.Id);
            return false;
        }

        var result = await _repo.UpdateAsync(user);
        if (result)
            _logger.LogInformation("User updated successfully with Id {UserId}", user.Id);
        else
            _logger.LogWarning("User update had no effect for Id {UserId}", user.Id);

        return result;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null)
        {
            _logger.LogWarning("User with Id {UserId} not found for deletion", id);
            return false;
        }

        var result = await _repo.DeleteAsync(existing);
        if (result)
            _logger.LogInformation("User deleted successfully with Id {UserId}", id);
        else
            _logger.LogWarning("User deletion had no effect for Id {UserId}", id);

        return result;
    }

    // ======================
    // PRIVATE MAPPER
    // ======================
    private  DtoUser ToDto(User user)
    {
        return new DtoUser
        {
            Id = user.Id,
            DataKey = user.DataKey,
            PersonID = user.PersonID,
            Role = user.Role,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}

