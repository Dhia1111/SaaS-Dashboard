using Connection.Data;
using Connection.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
public class DtoPerson
{
    public int Id { get; set; }
    public int DataKey { get; set; }
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string FirstName { get; set; } = null!;
    public string? LastName { get; set; }
    public string? Adress { get; set; }


}


public interface IPersonRepository
{
    Task<IReadOnlyList<Person>> GetAllAsync();

    Task<Person?> GetByIdAsync(int id);

    Task<Person?> GetByEmailAsync(string email);

    Task<IReadOnlyList<Person>> SearchByNameAsync(string namePattern);

    Task <bool> AddAsync(Person person);

    Task <bool> UpdateAsync(Person person);

    Task <bool> DeleteAsync(Person person);
}

 

public class clsPersonRepo : IPersonRepository
{
    private readonly SaasDashboardContext _context;
    private readonly ILogger<clsPersonRepo> _logger;

    public clsPersonRepo(SaasDashboardContext context, ILogger<clsPersonRepo> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IReadOnlyList<Person>> GetAllAsync()
    {
        try
        {
            return await _context.Persons.AsNoTracking().ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all persons");
            throw;
        }
    }

    public async Task<Person?> GetByIdAsync(int id)
    {
        try
        {
            return await _context.Persons.FindAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching person by Id {Id}", id);
            throw;
        }
    }

    public async Task<Person?> GetByEmailAsync(string email)
    {
        try
        {
            return await _context.Persons
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.Email == email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching person by Email {Email}", email);
            throw;
        }
    }

    public async Task<IReadOnlyList<Person>> GetByDataKeylAsync(int DataKey)
    {
        try
        {
            return await _context.Persons
                .Where(p => p.DataKey == DataKey)
                .AsNoTracking()
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching persons by DataKey {DataKey}", DataKey);
            throw;
        }
    }

    public async Task<IReadOnlyList<Person>> SearchByNameAsync(string namePattern)
    {
        try
        {
            return await _context.Persons
                .Where(p =>
                    EF.Functions.ILike(p.FirstName, $"%{namePattern}%") ||
                    EF.Functions.ILike(p.LastName!, $"%{namePattern}%"))
                .AsNoTracking()
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching persons by name pattern {Pattern}", namePattern);
            throw;
        }
    }

    public async Task<bool> AddAsync(Person person)
    {
        try
        {
            _context.Persons.Add(person);
            var result = await _context.SaveChangesAsync();
            _logger.LogInformation("Person added successfully with Id {Id}", person.Id);
            return result > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding person with Email {Email}", person.Email);
            return false;
        }
    }

    public async Task<bool> UpdateAsync(Person person)
    {
        try
        {
            _context.Persons.Update(person);
            var result = await _context.SaveChangesAsync();
            _logger.LogInformation("Person updated successfully with Id {Id}", person.Id);
            return result > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating person with Id {Id}", person.Id);
            return false;
        }
    }

    public async Task<bool> DeleteAsync(Person person)
    {
        try
        {
            _context.Persons.Remove(person);
            var result = await _context.SaveChangesAsync();
            _logger.LogInformation("Person deleted successfully with Id {Id}", person.Id);
            return result > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting person with Id {Id}", person.Id);
            return false;
        }
    }
}
