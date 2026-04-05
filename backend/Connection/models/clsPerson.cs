using Connection.Data;
using Connection.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
public class DtoPerson
{
    public int Id { get; set; }
    public int DataKey { get; set; }
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string? FirstName { get; set; } 
    public string? LastName { get; set; }
    public string? Address { get; set; }
    public string tokenHash { get; set; } = null!;
    public string EmailVerificationCodeExpiry { get; set; } = null!;

    [Required]
    public bool IsEmailVeryfied { get; set; }


}


public interface IPersonRepository:IGenericRepo<Person>
{


    Task<Person?> GetByEmailAsync(string email);

    Task<IReadOnlyList<Person>> SearchByNameAsync(string namePattern);
      Task<Person?> FindBySecureCodeAsync(string secureCode);

  
}

 

public class clsPersonRepo : GenericRepo<Person>,IPersonRepository
{

    public clsPersonRepo(SaasDashboardContext context, ILogger<clsPersonRepo> logger):base(context,logger)
    {
      
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
  public async Task<Person?> FindBySecureCodeAsync(string secureCode)
    {
        try
        {
            return await _context.Persons
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.SecureCode == secureCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching person by SecureCode {SecureCode}", secureCode);
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

    
  
  
}
