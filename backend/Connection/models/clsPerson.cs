using Connection.Data;
using Connection.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
public class DtoPerson
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string? FirstName { get; set; } 
    public string? LastName { get; set; }
    public string? Address { get; set; }
    public string? SecureCode { get; set; } 
    public string? EmailVerificationCodeExpiry { get; set; } 
    public string? ProviderId { get; set; } 
    public int? Provider { get; set; }

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
        { //AQAAAAIAAYagAAAAEGfTKE/gtSS7mN5X1caFym3fZFhfA4gQ164u5cRt1mg+t9hVg9SPM+Wqy9Sn2/gVLg==
          //AQAAAAIAAYagAAAAEEALywEFNAKgPG0shYJm+foXfDSlc09sdWWSKmtzU6CfRXYzdwq/wj/vWF76b96bEA==
            _logger.LogError(ex, "Error fetching person by SecureCode {SecureCode}", secureCode);
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
